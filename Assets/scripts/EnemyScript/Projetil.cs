// Gerencia o comportamento do projétil inimigo, incluindo movimento, colisão e animação de impacto
using UnityEngine;

public class Projetil : MonoBehaviour
{
    [Header("Configurações")]
    public float speed = 6f;
    public float lifeTime = 3f;
    public int damage = 15;
    public LayerMask hitLayers;
    public float impactAnimDuration = 0.4f; // ajustar no inspector conforme a duração real do clip "Hit"

    private Vector2 direction;
    private Animator animator;
    private bool hasHit = false;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.flipX = direction.x < 0f;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (!hasHit)
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return; // já explodiu, ignora colisões extras

        if (((1 << other.gameObject.layer) & hitLayers) != 0)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);

            hasHit = true;

            GetComponent<Collider2D>().enabled = false; // para de detectar mais colisões

            if (animator != null)
            {
                animator.SetTrigger("Hit");
                Destroy(gameObject, impactAnimDuration); // destrói só depois da explosão terminar
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}