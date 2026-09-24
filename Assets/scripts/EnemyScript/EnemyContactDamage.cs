using UnityEngine;


public class EnemyContactDamage : MonoBehaviour
{
    [Header("Configuracoes")]
    public int contactDamage = 10; // Dano que o inimigo causa ao tocar

    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o que encostou tem a Tag "Player"
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(contactDamage);
            }
        }
    }
}