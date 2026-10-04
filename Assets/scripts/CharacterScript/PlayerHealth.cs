// Gerencia a vida do jogador, incluindo dano, morte e feedback visual
using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Modificadores de combate (upgrades)")]
    public float damageReductionPercent = 0f;
    public float dodgeChancePercent = 0f;
    public int healOnKillAmount = 0;

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

        if (PlayerHUD.Instance != null)
        {
            PlayerHUD.Instance.UpdateHealth(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || isDead) return;

        // Chance de esquiva
        if (dodgeChancePercent > 0f && Random.Range(0f, 100f) < dodgeChancePercent)
        {
            StartCoroutine(InvincibilityRoutine());
            return;
        }

        // Redução percentual de dano
        if (damageReductionPercent > 0f)
        {
            damage = Mathf.RoundToInt(damage * (1f - Mathf.Clamp(damageReductionPercent, 0f, 100f) / 100f));
        }

        currentHealth -= damage;

        if (AudioManager.Instance != null && AudioManager.Instance.playerHitClip != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerHitClip);
        }

        if (PlayerHUD.Instance != null)
        {
            PlayerHUD.Instance.UpdateHealth(currentHealth, maxHealth);
        }

        if (currentHealth > 0 && animator != null)
        {
            animator.SetTrigger("Hurt");
        }

        StartCoroutine(FlashRed());
        StartCoroutine(InvincibilityRoutine());

        if (currentHealth <= 0) Die();
    }

    // Chamado pelo EnemyHealth quando esse player derrota um inimigo
    public void OnEnemyKilled()
    {
        if (healOnKillAmount <= 0) return;

        currentHealth = Mathf.Min(currentHealth + healOnKillAmount, maxHealth);

        if (PlayerHUD.Instance != null)
            PlayerHUD.Instance.UpdateHealth(currentHealth, maxHealth);
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

    public void SetDashInvincibility(bool value)
    {
        isInvincible = value;
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player morreu!");

        ValinaController valina = GetComponent<ValinaController>();
        if (valina != null) valina.enabled = false; // desativa o controle da Valina ao morrer

        DoroController doro = GetComponent<DoroController>();
        if (doro != null) doro.enabled = false; // desativa o controle da Doro ao morrer

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

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount; // cura a diferença também, pra não sobrar barra vazia proporcionalmente

        if (PlayerHUD.Instance != null)
        {
            PlayerHUD.Instance.UpdateHealth(currentHealth, maxHealth);
        }
    }

    // Poção da Valina: dobra a vida máxima e atual na mesma proporção
    public void MultiplyMaxHealth(float multiplier)
    {
        maxHealth = Mathf.RoundToInt(maxHealth * multiplier);
        currentHealth = Mathf.Min(Mathf.RoundToInt(currentHealth * multiplier), maxHealth);

        if (PlayerHUD.Instance != null)
            PlayerHUD.Instance.UpdateHealth(currentHealth, maxHealth);
    }

    public void AddDamageReduction(float percent)
    {
        damageReductionPercent = Mathf.Clamp(damageReductionPercent + percent, 0f, 100f);
    }

    public void AddDodgeChance(float percent)
    {
        dodgeChancePercent = Mathf.Clamp(dodgeChancePercent + percent, 0f, 100f);
    }

    public void AddHealOnKill(int amount)
    {
        healOnKillAmount += amount;
    }

    public void ResetAfterDeath()
    {
        isDead = false;
        currentHealth = maxHealth;

        if (PlayerHUD.Instance != null)
        {
            PlayerHUD.Instance.UpdateHealth(currentHealth, maxHealth);
            PlayerHUD.Instance.ResetXPKeepLevel();
        }

        if (CheckpointManager.Instance != null)
        {
            transform.position = CheckpointManager.Instance.GetCheckpoint();
        }

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