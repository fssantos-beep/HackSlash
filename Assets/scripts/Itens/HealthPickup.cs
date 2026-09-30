// Script para o item de cura no chão
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Cura")]
    public int healAmount = 30;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        // não deixa pegar o item se a vida já estiver cheia
        if (playerHealth.currentHealth >= playerHealth.maxHealth) return;

        playerHealth.currentHealth = Mathf.Min(playerHealth.currentHealth + healAmount, playerHealth.maxHealth);

        if (PlayerHUD.Instance != null)
            PlayerHUD.Instance.UpdateHealth(playerHealth.currentHealth, playerHealth.maxHealth);

        Destroy(gameObject);
    }
}