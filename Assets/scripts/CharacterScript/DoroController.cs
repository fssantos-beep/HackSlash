using UnityEngine;
using System.Collections;

public class DoroController : MonoBehaviour, IAttackable, IUpgradable
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

    [Header("Special Attack (Flecha)")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    public int specialAttackDamage = 40;
    public float specialAttackCooldown = 1.5f;
    private float nextSpecialAttackTime = 0f;
    private int currentAttackDamage;

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

        if (spriteRenderer != null && horizontalInput != 0 && !isAttacking)
        {
            facingRight = horizontalInput > 0;
            spriteRenderer.flipX = horizontalInput > 0; // Espelha ao virar pra direita

            if (attackPoint != null)
            {
                Vector3 attackPos = attackPoint.localPosition;
                attackPos.x = Mathf.Abs(attackPos.x) * (facingRight ? 1 : -1);
                attackPoint.localPosition = attackPos;
            }

            // O firePoint precisa virar junto, senão a flecha sempre sai pro mesmo lado
            if (firePoint != null)
            {
                Vector3 firePos = firePoint.localPosition;
                firePos.x = Mathf.Abs(firePos.x) * (facingRight ? 1 : -1);
                firePoint.localPosition = firePos;
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

        if (Input.GetKey(KeyCode.Q) && !isAttacking && !isDashing && Time.time >= nextSpecialAttackTime)
        {
            StartCoroutine(SpecialAttackRoutine());
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isAttacking && !isDashing && Time.time >= nextDashTime)
        {
            StartCoroutine(DashRoutine());
        }

        // Atualiza o HUD todo frame com quanto falta pro Special Attack ficar disponível de novo
        if (PlayerHUD.Instance != null)
        {
            float remaining = Mathf.Max(0f, nextSpecialAttackTime - Time.time);
            PlayerHUD.Instance.UpdateSpecialAttackCooldown(remaining, specialAttackCooldown);
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
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        currentAttackDamage = attackDamage;
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.5f); // Duração aproximada da animação de ataque

        isAttacking = false;
    }

    IEnumerator SpecialAttackRoutine()
    {
        isAttacking = true;
        nextSpecialAttackTime = Time.time + specialAttackCooldown;

        animator.SetTrigger("SpecialAttack");

        yield return new WaitForSeconds(0.8f);

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

    // Chamado via Animation Event no frame de impacto do ataque basico
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
                enemyHealth.TakeDamage(currentAttackDamage);

                if (enemyHealth.currentHealth <= 0 && PlayerHUD.Instance != null)
                {
                    PlayerHUD.Instance.AddXP(10);
                }
            }
        }
    }

    // Chamado via Animation Event no frame exato em que o arco solta a flecha
    public void FireArrow()
    {
        if (arrowPrefab == null || firePoint == null) return;

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        PlayerProjetil projetil = arrow.GetComponent<PlayerProjetil>();
        if (projetil != null)
        {
            projetil.damage = specialAttackDamage;
            projetil.hitLayers = enemyLayers;
            projetil.SetDirection(new Vector2(facingRight ? 1f : -1f, 0f));
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
    
    // Aplica o efeito do item escolhido na tela de level-up
    public void ApplyUpgrade(ItemData.EffectType effectType, float value)
    {
        switch (effectType)
        {
            case ItemData.EffectType.AttackDamage:
                attackDamage += (int)value;
                break;

            case ItemData.EffectType.MoveSpeed:
                velocidade += value;
                break;

            case ItemData.EffectType.AttackRange:
                attackRange += value;
                break;

            case ItemData.EffectType.MaxHealth:
                PlayerHealth playerHealth = GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.IncreaseMaxHealth((int)value);
                }
                break;
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