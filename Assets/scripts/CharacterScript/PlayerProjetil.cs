using UnityEngine;

public class PlayerProjetil : MonoBehaviour
{
    [Header("Configurações")]
    public float speed = 10f;
    public float lifeTime = 3f;
    public int damage = 40;
    public LayerMask hitLayers; // marque aqui as Layers dos inimigos (Enemy/EnemyBody)

    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.flipX = direction.x < 0f;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & hitLayers) != 0)
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);

                if (enemyHealth.currentHealth <= 0 && PlayerHUD.Instance != null)
                    PlayerHUD.Instance.AddXP(10);
            }
            Destroy(gameObject);
        }
    }
}