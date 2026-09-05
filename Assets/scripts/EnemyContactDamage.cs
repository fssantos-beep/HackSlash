using UnityEngine;


public class EnemyContactDamage : MonoBehaviour
{
    [Header("Configuracoes")]
    public int contactDamage = 10; // Dano que o inimigo causa ao tocar

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se o que encostou tem a Tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(contactDamage);
            }
        }
    }
}