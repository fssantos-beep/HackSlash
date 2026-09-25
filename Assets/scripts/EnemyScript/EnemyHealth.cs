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
    private SpriteRenderer spriteRenderer;

    [Header("Animação")]
    private Animator animator;
    public float deathAnimDuration = 1f; // ajustar conforme a duração real do clip "death"

    void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (AudioManager.Instance != null && AudioManager.Instance.enemyHitClip != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyHitClip);
        }

        // Avisa a barra de vida para atualizar
        EnemyHealthBar bar = GetComponentInChildren<EnemyHealthBar>();
        if (bar != null) bar.UpdateBar();

        if (currentHealth > 0 && animator != null)
            animator.SetTrigger("Hurt");

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
    if (PlayerHUD.Instance != null)
        PlayerHUD.Instance.AddXP(xpReward);
        Debug.Log($"{gameObject.name} morreu e deu {xpReward} XP!");

        EnemyMovement movement = GetComponent<EnemyMovement>();
        if (movement != null) movement.Kill();

        if (animator != null)
        {
            animator.SetTrigger("Die");
            // O Destroy agora é chamado pelo Animation Event, exatamente quando a animação acaba
        }
        else
        {
            Destroy(gameObject);
        }
    }

// Chamado via Animation Event, no último frame do clip "death"
    public void AnimationEvent_MorteCompleta()
    {
    Destroy(gameObject);
    }
}