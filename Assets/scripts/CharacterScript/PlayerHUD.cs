using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD Instance;

    [Header("Referências UI")]
    public Slider healthBar;
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI levelText;

    [Header("Curva de XP")]
    [Tooltip("Quanto maior, mais rápido o XP necessário cresce por nível")]
    public float xpCurveExponent = 1.5f;
    [Tooltip("Base usada no cálculo do XP necessário pra cada nível")]
    public int xpCurveBase = 100;

    private int currentXP = 0;
    private int currentLevel = 1;
    private int xpToNextLevel;

    void Awake()
    {
        // Garante que só existe um HUD e o registra
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Inicializa a barra com o valor máximo para não aparecer vazia no início
        if (healthBar != null)
        {
            healthBar.maxValue = 100; // Valor padrão, será sobrescrito pelo PlayerHealth
            healthBar.value = 100;
        }

        xpToNextLevel = CalculateXPForLevel(currentLevel);
        UpdateXPText();
        UpdateLevelText();
    }

    // Chamado pelo PlayerHealth quando ele toma dano ou se cura
    public void UpdateHealth(int current, int max)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = max;
            healthBar.value = current;
        }
    }

    // Chamado pelo Inimigo quando ele morre. Assinatura não muda, então quem já
    // chama isso (DoroController, por exemplo) continua funcionando sem ajuste.
    public void AddXP(int amount)
    {
        currentXP += amount;

        // Loop em vez de "if", pra cobrir o caso de um XP grande de uma vez só
        // levar a pessoa a subir mais de um nível de uma vez
        while (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }

        UpdateXPText();
    }

    private void LevelUp()
    {
        currentXP -= xpToNextLevel;
        currentLevel++;
        xpToNextLevel = CalculateXPForLevel(currentLevel);

        UpdateLevelText();

        if (ItemSelectionUI.Instance != null)
        {
            ItemSelectionUI.Instance.ShowItemChoices();
        }
    }

    // XP necessário cresce com o nível, pra ficar mais difícil upar quanto mais alto o nível
    private int CalculateXPForLevel(int level)
    {
        return Mathf.RoundToInt(xpCurveBase * Mathf.Pow(level, xpCurveExponent));
    }

    private void UpdateXPText()
    {
        if (xpText != null)
        {
            xpText.text = $"{currentXP} / {xpToNextLevel}";
        }
    }

    private void UpdateLevelText()
    {
        if (levelText != null)
        {
            levelText.text = "Nv. " + currentLevel;
        }
    }
}