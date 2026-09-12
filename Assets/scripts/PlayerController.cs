using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour, IAttackable
{
    [Header("Referências")]
    public Rigidbody2D rb;
    public Animator animator;
    public Transform attackPoint;      // ponto onde o ataque é desenhado
    public Transform groundCheck;      // ponto usado so pra saber se o pe ta tocando o chao
    public SpriteRenderer spriteRenderer;

    [Header("Camadas")]
    public LayerMask groundLayer;   // o que conta como "chao" pro OverlapCircle
    public LayerMask enemyLayers;   // o que conta como "inimigo" pro dano

    [Header("Movimentação")]
    public float moveSpeed = 7f;
    private float horizontalInput;     // -1, 0 ou 1, vindo do Input.GetAxisRaw
    private bool facingRight = true;   // pra saber pra que lado o personagem olha

    [Header("Pulo")]
    public float jumpForce = 12f;
    public float groundCheckRadius = 0.2f;
    private bool isGrounded;

    [Header("Ataque Básico")]
    public float basicAttackDamage = 10f;
    public float basicAttackRange = 0.5f;
    public float attackCooldown = 0.2f;              // tempo minimo entre ataques normais
    public float attackCooldownAfterDash = 0.3f;     // cooldown maior após o dash, pra evitar spam saindo dele
    private float nextAttackTime = 0f;

    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 0.5f;
    private float nextDashTime = 0f;
    private bool isDashing = false;

    [Header("Dash Attack")]
    public float dashAttackDamage = 20f;
    public float dashAttackRange = 0.8f;
    private bool dashAttackActivated = false;  // vira true se o jogador apertar ataque NO MEIO do dash

    private bool isAttacking = false;

    // Evita que o mesmo inimigo tome dano várias vezes num único ataque
    // (sem isso, um dash de 0.3s podia acertar o mesmo bicho várias vezes por causa do FixedUpdate/Update)
    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Vira o sprite e o ponto de ataque pro lado que o jogador está andando
        if (spriteRenderer != null && horizontalInput != 0)
        {
            facingRight = horizontalInput > 0;
            spriteRenderer.flipX = !facingRight;

            // Espelha o X do attackPoint pra garantir que o ataque saia na frente do personagem
            if (attackPoint != null)
            {
                Vector3 attackPos = attackPoint.localPosition;
                attackPos.x = Mathf.Abs(attackPos.x) * (facingRight ? 1 : -1);
                attackPoint.localPosition = attackPos;
            }
        }

        // Checagem de chao via um circulo invisivel embaixo do personagem
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        animator.SetBool("isGrounded", isGrounded);

        // PULO
        // Só pula se estiver no chao e nao estiver travado atacando ou dando dash
        if (Input.GetButtonDown("Jump") && isGrounded && !isAttacking && !isDashing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetTrigger("Jump");
        }

        if (Input.GetMouseButtonDown(0))
        {
            // Se ja estamos dando dash e ainda nao ativamos o dash attack,
            // isso vira um "ataque especial" saindo do dash
            if (isDashing && !dashAttackActivated)
            {
                dashAttackActivated = true;
                animator.SetTrigger("DashAttack");
            }
            else if (!isAttacking && !isDashing && Time.time >= nextAttackTime)
            {
                StartCoroutine(BasicAttackRoutine());
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isAttacking && !isDashing && Time.time >= nextDashTime)
        {
            StartCoroutine(DashRoutine());
        }
    }

    void FixedUpdate()
    {
        if (!isAttacking && !isDashing)
        {
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        }
        else if (isAttacking)
        {
            // Trava o movimento horizontal enquanto ataca, pra não "deslizar atacando"
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
        // Durante o dash a velocidade é controlada direto na DashRoutine

        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
    }

    IEnumerator BasicAttackRoutine()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;
        hitEnemies.Clear(); // comeca um ataque novo, entao limpa quem ja foi atingido antes
        animator.SetTrigger("Attack");
 
        // Duração fixa pra combinar com a animação. Se a animação mudar de tempo,
        // ajustar aqui também (idealmente isso viria do próprio clip)
        yield return new WaitForSeconds(0.5f);
 
        isAttacking = false;
    }

    IEnumerator DashRoutine()
    {
        isDashing = true;
        dashAttackActivated = false;
        nextDashTime = Time.time + dashCooldown;
        hitEnemies.Clear();

        animator.SetTrigger("Dash");

        float dashDirection = facingRight ? 1f : -1f;
        float dashEndTime = Time.time + dashDuration;

        // Empurra o personagem na direcao do dash frame a frame ate acabar o tempo
        while (Time.time < dashEndTime)
        {
            rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y);

            // Se o jogador apertou ataque durante o dash, vai aplicando dano
            // continuamente (o hitEnemies garante que cada inimigo so leva uma vez)
            if (dashAttackActivated)
            {
                ApplyDashAttackDamage();
            }

            yield return null;
        }

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        isDashing = false;
        dashAttackActivated = false;
        nextAttackTime = Time.time + attackCooldownAfterDash;
        hitEnemies.Clear();
    }

    // Chamado via Animation Event no frame de impacto do ataque básico
    public void OnAttackImpact()
    {
        // Se o dash attack estiver ativo, usa os valores dele em vez do ataque normal
        // isso cobre o caso de o Animation Event do ataque comum disparar durante um dash attack
        float currentDamage = dashAttackActivated ? dashAttackDamage : basicAttackDamage;
        float currentRange = dashAttackActivated ? dashAttackRange : basicAttackRange;

        Collider2D[] hitEnemiesInRange = Physics2D.OverlapCircleAll(attackPoint.position, currentRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemiesInRange)
        {
            if (!hitEnemies.Contains(enemy))
            {
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage((int)currentDamage);
                    hitEnemies.Add(enemy); // marca esse inimigo como "ja tomou dano"
                }
            }
        }
    }

    private void ApplyDashAttackDamage()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(attackPoint.position, dashAttackRange, enemyLayers);

        foreach (Collider2D enemy in enemiesInRange)
        {
            if (!hitEnemies.Contains(enemy))
            {
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage((int)dashAttackDamage);
                    hitEnemies.Add(enemy);
                }
            }
        }
    }

    // So pra debug visual no Editor, mostra os raios de ataque e a area de chao
    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, Mathf.Max(basicAttackRange, dashAttackRange));
        }
        if (groundCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}