using UnityEngine;
using System.Collections;

public class DoroController : MonoBehaviour, IAttackable
{
    [Header("Referências")]
    public Rigidbody2D rb;
    public Animator animator;
    public Transform attackPoint;
    public Transform checkGround;
    public SpriteRenderer spriteRenderer;

    [Header("Camadas")]
    public LayerMask groundLayer;
    public LayerMask enemyLayers;

    [Header("Movimentação")]
    public float velocidade = 5f;
    private float horizontalInput;
    private bool facingRight = false; // Sprite base olha para a esquerda

    [Header("Pulo")]
    public float forcaPulo = 10f;
    public float checkRadius = 0.1f;
    private bool isGrounded;

    [Header("Ataque")]
    public int attackDamage = 20;
    public float attackRange = 1f;
    public float attackCooldown = 0.5f;
    private float nextAttackTime = 0f;
    private bool isAttacking = false;

    [Header("Dash")]
    public float forcaDash = 15f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;
    private bool isDashing = false;
    private float nextDashTime = 0f;

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Só vira o sprite se não estiver atacando, pra não trocar de lado no meio do golpe
        if (spriteRenderer != null && horizontalInput != 0 && !isAttacking)
        {
            facingRight = horizontalInput > 0;
            spriteRenderer.flipX = horizontalInput > 0; // Espelha ao virar pra direita

            // Reposiciona o attackPoint pro lado que a Doro está olhando
            if (attackPoint != null)
            {
                Vector3 attackPos = attackPoint.localPosition;
                attackPos.x = Mathf.Abs(attackPos.x) * (facingRight ? 1 : -1);
                attackPoint.localPosition = attackPos;
            }
        }

        isGrounded = Physics2D.OverlapCircle(checkGround.position, checkRadius, groundLayer);
        animator.SetBool("isGrounded", isGrounded);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isAttacking && !isDashing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);
            animator.SetTrigger("Jump");
        }

        if (Input.GetMouseButtonDown(0) && !isAttacking && !isDashing && Time.time >= nextAttackTime)
        {
            StartCoroutine(AttackRoutine());
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
            rb.linearVelocity = new Vector2(horizontalInput * velocidade, rb.linearVelocity.y);
        }
        else if (isAttacking)
        {
            // Trava o movimento horizontal enquanto ataca
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        // Mesmo parâmetro "Speed" usado no PlayerController, pra manter o Animator consistente
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.5f); // Duração aproximada da animação de ataque

        isAttacking = false;
    }

    IEnumerator DashRoutine()
    {
        isDashing = true;
        nextDashTime = Time.time + dashCooldown;

        animator.SetTrigger("Dash");

        float dashDirection = facingRight ? 1f : -1f;
        float dashEndTime = Time.time + dashTime;

        while (Time.time < dashEndTime)
        {
            rb.linearVelocity = new Vector2(dashDirection * forcaDash, rb.linearVelocity.y);
            yield return null;
        }

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        isDashing = false;
    }

    // Chamado via Animation Event no frame de impacto do golpe
    public void OnAttackImpact()
    {
        if (attackPoint == null)
        {
            Debug.LogError("DoroController: AttackPoint não configurado!");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);

                if (enemyHealth.currentHealth <= 0 && PlayerHUD.Instance != null)
                {
                    PlayerHUD.Instance.AddXP(10);
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
        if (checkGround != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(checkGround.position, checkRadius);
        }
    }
}