// Gerencia a vida do jogador, incluindo dano, morte e feedback visual
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
    private bool isDead = false;

    [Header("Checkpoint")]
    public static PlayerHealth Instance;

    [Header("Animação")]
    public Animator animator;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

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
        if (isInvincible || isDead) return;

        currentHealth -= damage;

        // Avisa o HUD para atualizar a barra
        if (PlayerHUD.Instance != null)
        {
            PlayerHUD.Instance.UpdateHealth(currentHealth, maxHealth);
        }

        // Só toca o hurt se sobreviveu ao golpe, pra não brigar com a animação de morte
        if (currentHealth > 0 && animator != null)
        {
            animator.SetTrigger("Hurt");
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
        isDead = true;
        Debug.Log("Player morreu!");

        ValinaController valina = GetComponent<ValinaController>();
        if (valina != null) valina.enabled = false; // Desativa o controle do jogador

        DoroController doro = GetComponent<DoroController>();
        if (doro != null) doro.enabled = false; // Desativa o controle do jogador

        if (animator != null)
        {
            animator.SetTrigger("Die");
            StartCoroutine(ShowDefeatAfterAnimation());
        }
        else
        {
            ShowDefeat(); // Se não houver animação, mostra a tela de derrota imediatamente
        }
    }

    IEnumerator ShowDefeatAfterAnimation()
    {
        yield return new WaitForSeconds(1f); // ajuste pra duração real da animação de morte
        ShowDefeat();
    }

    void ShowDefeat()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ShowDefeat();
    }

    // Chamado pelo item de upgrade de vida máxima na tela de level-up
    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount; // cura a diferença também, pra não sobrar barra vazia proporcionalmente

        if (PlayerHUD.Instance != null)
        {
            PlayerHUD.Instance.UpdateHealth(currentHealth, maxHealth);
        }
    }

    public void ResetAfterDeath()
    {
        isDead = false;
        currentHealth = maxHealth;

        if (PlayerHUD.Instance != null)
            PlayerHUD.Instance.UpdateHealth(currentHealth, maxHealth);

        if (CheckpointManager.Instance != null)
            transform.position = CheckpointManager.Instance.GetCheckpoint();

        ValinaController valina = GetComponent<ValinaController>();
        if (valina != null) valina.enabled = true;

        DoroController doro = GetComponent<DoroController>();
        if (doro != null) doro.enabled = true;

        if (animator != null)
        {
            animator.Rebind();      // volta o Animator pro estado inicial (idle)
            animator.Update(0f);
        }
    }
}