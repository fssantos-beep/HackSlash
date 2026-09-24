using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movimento aleatório")]
    public float moveSpeed = 2f;
    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;
    public float minWalkTime = 1f;
    public float maxWalkTime = 2.5f;

    [Header("Ataque")]
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;
    public Transform target;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float moveDirection = 0f; // -1 esquerda, 0 parado, 1 direita
    private float lastAttackTime = -999f;
    private bool isDead = false;

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
        // Continua procurando pelo Player até ele existir na cena
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

        if (moveDirection != 0f)
        {
            Vector2 newPos = new Vector2(
                rb.position.x + moveDirection * moveSpeed * Time.deltaTime,
                rb.position.y
            );
            rb.MovePosition(newPos);
            FlipSprite(moveDirection);
        }

        if (target != null && animator != null)
        {
            float distance = Vector2.Distance(transform.position, target.position);
            if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
            {
                animator.SetTrigger("attack");
                lastAttackTime = Time.time;
            }
        }
    }

    IEnumerator BehaviorLoop()
    {
        while (!isDead)
        {
            // Fica parado por um tempo aleatório
            moveDirection = 0f;
            if (animator != null) animator.SetBool("IsMoving", false);
            yield return new WaitForSeconds(Random.Range(minIdleTime, maxIdleTime));

            if (isDead) yield break;

            // Anda numa direção aleatória por um tempo aleatório
            moveDirection = Random.value > 0.5f ? 1f : -1f;
            if (animator != null) animator.SetBool("IsMoving", true);
            yield return new WaitForSeconds(Random.Range(minWalkTime, maxWalkTime));
        }
    }

    void FlipSprite(float directionX)
    {
        if (spriteRenderer == null) return;
        if (directionX > 0f) spriteRenderer.flipX = false;
        else if (directionX < 0f) spriteRenderer.flipX = true;
    }

    public void Kill()
    {
        isDead = true;
        moveDirection = 0f;
        StopAllCoroutines();
    }
}