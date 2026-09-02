using UnityEngine;
using System.Collections;


public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 30;
    private int currentHealth;

    [Header("Feedback Visual")]
    public float flashDuration = 0.15f;   // Quanto tempo fica vermelho
    public Color hitColor = Color.red;    // Cor ao tomar dano
    private SpriteRenderer spriteRenderer;
    private Color originalColor;          // Guarda a cor original pra voltar depois

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        // Salva a cor original do sprite
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    // Chamado pelo PlayerController quando o player acerta o inimigo
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} recebeu {damage} de dano! Vida: {currentHealth}/{maxHealth}");

        // Toca o flash vermelho
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Flash vermelho: pinta o sprite de vermelho e volta pra cor original
    IEnumerator FlashRed()
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }
    
    void Die()
    {
        Debug.Log($"{gameObject.name} morreu!");
        Destroy(gameObject);
    }
}