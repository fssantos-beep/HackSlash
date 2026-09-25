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

    [Header("Detecção do jogador")]
    public float detectionRange = 6f; // raio em que o inimigo "percebe" o player e entra em combate

    [Header("Ataque")]
    public float attackRange = 1f;
    public float attackCooldown = 1.5f;
    public Transform target;

    [HideInInspector]
    public float facingDirection = 1f; // usado pelo EnemyRangedAttack pra saber pra que lado atirar

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float moveDirection = 0f; // -1 esquerda, 0 parado, 1 direita
    private float lastAttackTime = -999f;
    private bool isDead = false;
    private bool inCombat = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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
            if (player != null)
            {
                target = player.transform;
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    void Update()
    {
        if (isDead) return;

        float distance = 0f;
        // Verifica se o player entrou ou saiu do raio de detecção
        if (target != null)
        {
            distance = Vector2.Distance(transform.position, target.position);
            inCombat = distance <= detectionRange;
        }

        if (inCombat)
        {
            float dirToTarget = target.position.x - transform.position.x;

            if (distance > attackRange)
            {
                float dir = Mathf.Sign(dirToTarget);
                Vector2 movement = new Vector2(rb.position.x + dir * moveSpeed * Time.deltaTime, rb.position.y);
                rb.MovePosition(movement);
                FlipSprite(dir);
                if (animator != null) animator.SetBool("IsMoving", true);
            }
            else
            {
                FlipSprite(dirToTarget);
                if (animator != null) animator.SetBool("IsMoving", false);
                if (Time.time >= lastAttackTime + attackCooldown && animator != null)
                {
                    animator.SetTrigger("attack");
                    lastAttackTime = Time.time;
                }
            }
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

        if (directionX > 0f)
        {
            spriteRenderer.flipX = false;
            facingDirection = 1f;
        }
        else if (directionX < 0f)
        {
            spriteRenderer.flipX = true;
            facingDirection = -1f;
        }
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
    }
}