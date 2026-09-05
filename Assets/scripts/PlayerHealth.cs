using UnityEngine;
using System.Collections;


public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Feedback Visual")]
    public float flashDuration = 0.15f;
    public Color hitColor = Color.red;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Invencibilidade")]
    public float invincibilityDuration = 0.5f;
    private bool isInvincible = false;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;

        // Avisa o HUD da vida inicial
        if (PlayerHUD.Instance != null)
        {
            PlayerHUD.Instance.UpdateHealth(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        
        // Avisa o HUD para atualizar a barra
        if (PlayerHUD.Instance != null)
        {
            PlayerHUD.Instance.UpdateHealth(currentHealth, maxHealth);
        }

        StartCoroutine(FlashRed());
        StartCoroutine(InvincibilityRoutine());

        if (currentHealth <= 0) Die();
    }

    IEnumerator FlashRed()
    {
        if (spriteRenderer == null) yield break;
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        float timer = 0f;
        while (timer < invincibilityDuration)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
            timer += 0.2f;
        }
        spriteRenderer.color = originalColor;
        isInvincible = false;
    }

    void Die()
    {
        Debug.Log("Player morreu!");
        gameObject.SetActive(false); // Desativa o player (futuro: tela de Game Over)
    }
}