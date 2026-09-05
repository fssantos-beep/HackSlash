using UnityEngine;
using System.Collections;


public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 30;
    public int currentHealth;

    [Header("Recompensa")]
    public int xpReward = 10; // Quanto XP esse inimigo da ao morrer (altera no inspector)

    [Header("Feedback Visual")]
    public float flashDuration = 0.15f;   // Quanto tempo fica vermelho
    public Color hitColor = Color.red;    // Cor ao tomar dano
    private SpriteRenderer spriteRenderer;
    private Color originalColor;          // Guarda a cor original pra voltar depois

    // faz o inimigo iniciar com vida full
    void Awake()
    {
        currentHealth = maxHealth;
        
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null) 
            originalColor = spriteRenderer.color;
    }

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        // Avisa a barra de vida para atualizar
        EnemyHealthBar bar = GetComponentInChildren<EnemyHealthBar>();
        if (bar != null)
        {
            bar.UpdateBar();
        }
        StartCoroutine(FlashRed());
        if (currentHealth <= 0) Die();
    }

    IEnumerator FlashRed()
    {
        if (spriteRenderer == null) yield break;
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        // Avisa o HUD para dar XP ao jogador
        if (PlayerHUD.Instance != null)
        {
            PlayerHUD.Instance.AddXP(xpReward);
        }

        Debug.Log($"{gameObject.name} morreu e deu {xpReward} XP!");
        Destroy(gameObject);
    }
}