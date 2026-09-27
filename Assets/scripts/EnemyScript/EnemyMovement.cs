// Gerencia o movimento do inimigo, incluindo patrulha aleatória, detecção do jogador e ataques corpo-a-corpo e à distância
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Patrulha aleatória")]
    public float moveSpeed = 2f;
    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;
    public float minWalkTime = 1f;
    public float maxWalkTime = 2.5f;

    [Header("Detecção do jogador (Amarelo)")]
    public float detectionRange = 6f;

    [Header("Ataque Principal (Attack1 / 'attack', range vermelho)")]
    public float attackCooldown = 1.5f;

    [Header("Ataque Secundário (Attack2, range Roxo)")]
    public bool hasSecondaryAttack = false;
    public float secondaryAttackRange = 5f;
    public float secondaryAttackCooldown = 3f;

    [Header("Pontos que precisam virar junto com o sprite")]
    public Transform firePoint;
    public Transform attackPoint;

    public Transform target;

    [HideInInspector]
    public float facingDirection = 1f; // 1f = olhando pra direita, -1f = olhando pra esquerda

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private EnemyMeleeAttack meleeAttack;

    private float moveDirection = 0f; // 1f = andando pra direita, -1f = andando pra esquerda, 0f = parado
    private float lastAttackTime = -999f;
    private float lastSecondaryAttackTime = -999f;
    private bool isDead = false;
    private bool inCombat = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        meleeAttack = GetComponent<EnemyMeleeAttack>();

        if (attackPoint == null && meleeAttack != null)
            attackPoint = meleeAttack.attackPoint;
    }

    void Start()
    {
        StartCoroutine(FindPlayerRoutine());
        StartCoroutine(BehaviorLoop());
    }

    IEnumerator FindPlayerRoutine()
    {
        while (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) { target = player.transform; yield break; }
            yield return new WaitForSeconds(0.5f);
        }
    }

    void Update()
    {
        if (isDead) return;
        float meleeRange = meleeAttack != null ? meleeAttack.attackRange : 1f;

        if (target != null)
        {
            float distance = Vector2.Distance(transform.position, target.position);
            inCombat = distance <= detectionRange;

            if (inCombat)
            {
                float dirToTarget = target.position.x - transform.position.x;
                FlipSprite(dirToTarget); // vira o sprite e os pontos de ataque na direção do player

                bool withinMeleeZone = distance <= meleeRange;
                bool withinRangedZone = hasSecondaryAttack && distance <= secondaryAttackRange;

                if (withinMeleeZone)
                {
                    // Vermelho: perto o suficiente pro corpo-a-corpo — prioridade máxima
                    moveDirection = 0f;
                    if (animator != null) animator.SetBool("IsMoving", false);

                    if (Time.time >= lastAttackTime + attackCooldown && animator != null)
                    {
                        animator.SetTrigger("attack");
                        lastAttackTime = Time.time;
                    }
                }
                else if (withinRangedZone)
                {
                    // Roxo: perto o suficiente pro projétil
                    moveDirection = 0f;
                    if (animator != null) animator.SetBool("IsMoving", false);

                    if (Time.time >= lastSecondaryAttackTime + secondaryAttackCooldown && animator != null)
                    {
                        animator.SetTrigger("attack2");
                        lastSecondaryAttackTime = Time.time;
                    }
                    else
                    {
                        // Projétil em cooldown: anda em direção ao player pra tentar o melee
                        float dir = Mathf.Sign(dirToTarget);
                        Vector2 newPos = new Vector2(rb.position.x + dir * moveSpeed * Time.deltaTime, rb.position.y);
                        rb.MovePosition(newPos);
                        if (animator != null) animator.SetBool("IsMoving", true);
                    }
                }
                else
                {
                    // Fora do alcance de ataque: anda em direção ao player se projetil estiver em cooldown
                    float dir = Mathf.Sign(dirToTarget);
                    Vector2 newPos = new Vector2(rb.position.x + dir * moveSpeed * Time.deltaTime, rb.position.y);
                    rb.MovePosition(newPos);
                    if (animator != null) animator.SetBool("IsMoving", true);
                }
                return;
            }
        }

        // Fora de combate: patrulha aleatória normal
        if (moveDirection != 0f)
        {
            Vector2 newPos = new Vector2(rb.position.x + moveDirection * moveSpeed * Time.deltaTime, rb.position.y);
            rb.MovePosition(newPos);
            FlipSprite(moveDirection);
        }
    }

    IEnumerator BehaviorLoop()
    {
        while (!isDead)
        {
            if (!inCombat)
            {
                moveDirection = 0f;
                if (animator != null) animator.SetBool("IsMoving", false);
                yield return new WaitForSeconds(Random.Range(minIdleTime, maxIdleTime));

                if (isDead || inCombat) continue;

                moveDirection = Random.value > 0.5f ? 1f : -1f;
                if (animator != null) animator.SetBool("IsMoving", true);
                yield return new WaitForSeconds(Random.Range(minWalkTime, maxWalkTime));
            }
            else
            {
                yield return null; // pausa a patrulha enquanto estiver em combate
            }
        }
    }

    void FlipSprite(float directionX)
    {
        if (spriteRenderer == null) return;

        bool shouldFaceLeft = directionX < 0f;
        spriteRenderer.flipX = shouldFaceLeft;
        facingDirection = shouldFaceLeft ? -1f : 1f;

        MirrorPoint(firePoint);
        MirrorPoint(attackPoint);
    }

    void MirrorPoint(Transform point)
    {
        if (point == null) return;
        Vector3 pos = point.localPosition;
        pos.x = Mathf.Abs(pos.x) * facingDirection;
        point.localPosition = pos;
    }

    public void Kill()
    {
        isDead = true;
        moveDirection = 0f;
        StopAllCoroutines();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (hasSecondaryAttack)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, secondaryAttackRange);
        }
    }
}