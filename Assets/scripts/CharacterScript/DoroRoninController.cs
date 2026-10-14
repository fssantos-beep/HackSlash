using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoroRoninController : MonoBehaviour, IAttackable, IUpgradable
{
    [Header("Referências")]
    public Rigidbody2D rb;
    public Animator animator;
    public Transform attackPoint;
    public Transform checkGround;
    public SpriteRenderer spriteRenderer;
    private PlayerHealth playerHealth;

    [Header("Camadas")]
    public LayerMask groundLayer;
    public LayerMask enemyLayers;

    [Header("Movimentação")]
    public float velocidade = 5f;
    private float horizontalInput;
    private bool facingRight = false; // Sprite base olha para a esquerda (ver spritesheet)

    [Header("Pulo")]
    public float forcaPulo = 10f;
    public float checkRadius = 0.1f;
    private bool isGrounded;

    [Header("Ataque Básico (katana)")]
    public int attackDamage = 18;
    public float attackRange = 1f;
    public float attackCooldown = 0.5f;
    private float nextAttackTime = 0f;
    private bool isAttacking = false;

    [Header("Dash (esquiva)")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    private bool isDashing = false;
    private float nextDashTime = 0f;

    [Header("Special Attack (Iaijutsu)")]
    public float iaiSpeed = 22f; // velocidade do dash do iai
    public float iaiDuration = 0.25f; // duração do dash do iai
    public int iaiDamage = 45; // dano total do iai (dividido entre os cortes)
    public float iaiRange = 1.2f; // alcance do iai (raio da detecção de inimigos)
    public float specialAttackCooldown = 2.5f; // tempo de recarga do iai
    
    public int iaiHits = 3; // número de cortes que o iai pode causar no inimigo atingido
    public float iaiHitInterval = 0.1f; // intervalo entre os cortes do iai

    //  Campos para controle da animação de cortes/desembainhar
    public float iaiSlashAnimDuration = 0.6f; // duração do clip SpecialSlash (cortes + guardar espada)
    public float iaiSlashDelay = 0f; // atraso até o 1º corte aparecer no clip

    private float nextSpecialAttackTime = 0f;
    private bool isUsingSpecial = false;

    [Header("UI")]
    public Sprite specialIcon;

    // Lista para armazenar inimigos atingidos pelo iai, evitando múltiplos hits no mesmo inimigo durante o mesmo ataque
    private readonly List<Collider2D> hitEnemiesSpecial = new List<Collider2D>();

    void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerHealth = GetComponent<PlayerHealth>();

        if (PlayerHUD.Instance != null && specialIcon != null)
        {
            PlayerHUD.Instance.SetSpecialIcon(specialIcon);
        }
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (spriteRenderer != null && horizontalInput != 0 && !isAttacking && !isUsingSpecial)
        {
            facingRight = horizontalInput > 0;
            spriteRenderer.flipX = horizontalInput > 0;

            if (attackPoint != null)
            {
                Vector3 attackPos = attackPoint.localPosition;
                attackPos.x = Mathf.Abs(attackPos.x) * (facingRight ? 1 : -1);
                attackPoint.localPosition = attackPos;
            }
        }

        isGrounded = Physics2D.OverlapCircle(checkGround.position, checkRadius, groundLayer);
        animator.SetBool("isGrounded", isGrounded);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isAttacking && !isDashing && !isUsingSpecial)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);
            animator.SetTrigger("Jump");
        }

        if (Input.GetMouseButtonDown(0) && !isAttacking && !isDashing && !isUsingSpecial && Time.time >= nextAttackTime)
        {
            StartCoroutine(AttackRoutine());
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isAttacking && !isDashing && !isUsingSpecial && Time.time >= nextSpecialAttackTime)
        {
            StartCoroutine(SpecialAttackRoutine());
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isAttacking && !isDashing && !isUsingSpecial && Time.time >= nextDashTime)
        {
            StartCoroutine(DashRoutine());
        }

        if (PlayerHUD.Instance != null)
        {
            float remaining = Mathf.Max(0f, nextSpecialAttackTime - Time.time);
            PlayerHUD.Instance.UpdateSpecialAttackCooldown(remaining, specialAttackCooldown);
        }
    }

    void FixedUpdate()
    {
        if (!isAttacking && !isDashing && !isUsingSpecial)
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

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
    }

    IEnumerator DashRoutine()
    {
        isDashing = true;
        nextDashTime = Time.time + dashCooldown;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.dodgeClip);
        }

        animator.SetTrigger("Dash");

        float dashDirection = facingRight ? 1f : -1f;
        float dashEndTime = Time.time + dashDuration;

        while (Time.time < dashEndTime)
        {
            rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y);
            yield return null;
        }

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        isDashing = false;
    }

    IEnumerator SpecialAttackRoutine()
    {
        isUsingSpecial = true;
        nextSpecialAttackTime = Time.time + specialAttackCooldown;

        Vector2 startPosition = rb.position; // guarda onde ela estava antes do golpe

        if (playerHealth != null)
            playerHealth.SetDashInvincibility(true);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.doroSpecialAttackClip);
        }

        // Limpa o trigger SpecialSlash antes de ativar o SpecialAttack
        animator.ResetTrigger("SpecialSlash");
        animator.SetTrigger("SpecialAttack");

        float dashDirection = facingRight ? 1f : -1f;
        float endTime = Time.time + iaiDuration;
        hitEnemiesSpecial.Clear();
        bool stoppedOnEnemy = false;

        // Fase 1: avança até encontrar um inimigo
        while (Time.time < endTime)
        {
            rb.linearVelocity = new Vector2(dashDirection * iaiSpeed, rb.linearVelocity.y);

            Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, iaiRange, enemyLayers);
            foreach (Collider2D hit in hits)
            {
                if (!hitEnemiesSpecial.Contains(hit))
                {
                    EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
                    if (enemyHealth != null && enemyHealth.currentHealth > 0)
                    {
                        hitEnemiesSpecial.Add(hit);
                        StartCoroutine(SpecialMultiHit(enemyHealth));
                        stoppedOnEnemy = true;
                    }
                }
            }

            if (stoppedOnEnemy) break;

            yield return null;
        }

        // Zera a velocidade e sincroniza a execução do SpecialSlash com a duração do clip
        rb.linearVelocity = Vector2.zero;
        if (stoppedOnEnemy)
        {
            animator.SetTrigger("SpecialSlash");
            yield return new WaitForSeconds(iaiSlashAnimDuration); // cortes + guardar a espada
        }

        // Fase 2: reaparece na posição original (efeito "afterimage")
        rb.position = startPosition;
        transform.position = startPosition;

        if (playerHealth != null)
            playerHealth.SetDashInvincibility(false);

        isUsingSpecial = false;
    }

    // Aplica o iaiSlashDelay no início para sincronizar o primeiro hit com a animação
    // e depois aplica os hits subsequentes com base no iaiHitInterval
    IEnumerator SpecialMultiHit(EnemyHealth enemyHealth)
    {
        if (iaiSlashDelay > 0f) yield return new WaitForSeconds(iaiSlashDelay);

        int hits = Mathf.Max(1, iaiHits);
        for (int i = 0; i < hits; i++)
        {
            if (enemyHealth != null && enemyHealth.currentHealth > 0)
            {
                enemyHealth.TakeDamage(iaiDamage);
                
                if (enemyHealth.currentHealth <= 0 && PlayerHUD.Instance != null)
                {
                    PlayerHUD.Instance.AddXP(10);
                    break; // Interrompe os cortes se o inimigo já morreu
                }
            }
            yield return new WaitForSeconds(iaiHitInterval);
        }
    }

    // Chamado via Animation Event no frame de impacto do ataque básico
    public void OnAttackImpact()
    {
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
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    public void ApplyUpgrade(ItemData.EffectType effectType, float value)
    {
        PlayerHealth ph;
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
                ph = GetComponent<PlayerHealth>();
                if (ph != null) ph.IncreaseMaxHealth((int)value);
                break;

            case ItemData.EffectType.AttackDamagePercent:
                attackDamage = Mathf.RoundToInt(attackDamage * (1f + value / 100f));
                break;

            case ItemData.EffectType.AttackSpeedPercent:
                attackCooldown = Mathf.Max(0.05f, attackCooldown * (1f - value / 100f));
                break;

            case ItemData.EffectType.SpecialCooldownPercent:
                specialAttackCooldown = Mathf.Max(0.1f, specialAttackCooldown * (1f - value / 100f));
                break;

            case ItemData.EffectType.SpecialAttackDamage:
                iaiDamage += (int)value;
                break;

            case ItemData.EffectType.DamageReductionPercent:
                ph = GetComponent<PlayerHealth>();
                if (ph != null) ph.AddDamageReduction(value);
                break;

            case ItemData.EffectType.DodgeChancePercent:
                ph = GetComponent<PlayerHealth>();
                if (ph != null) ph.AddDodgeChance(value);
                break;

            case ItemData.EffectType.HealOnKill:
                ph = GetComponent<PlayerHealth>();
                if (ph != null) ph.AddHealOnKill((int)value);
                break;

            case ItemData.EffectType.MaxHealthMultiplier:
                ph = GetComponent<PlayerHealth>();
                if (ph != null) ph.MultiplyMaxHealth(value);
                break;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, Mathf.Max(attackRange, iaiRange));
        }
        if (checkGround != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(checkGround.position, checkRadius);
        }
    }
}