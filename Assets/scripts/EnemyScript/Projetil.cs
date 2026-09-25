using UnityEngine;

public class Projetil : MonoBehaviour
{
    [Header("Configurações")]
    public float speed = 6f;
    public float lifeTime = 3f;
    public int damage = 15;
    public LayerMask hitLayers;

    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipX = direction.x < 0;
        }
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
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);

            Destroy(gameObject); // some ao acertar player OU chão
        }
    }
}