using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerHUD : MonoBehaviour
{
    // Singleton: Permite que outros scripts acessem este HUD facilmente
    public static PlayerHUD Instance; 

    [Header("Referências UI")]
    public Slider healthBar;
    public TextMeshProUGUI xpText; // Se estiver usando TextMeshPro, mude para TextMeshProUGUI

    private int currentXP = 0;

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
        UpdateXPText();
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

    // Chamado pelo Inimigo quando ele morre
    public void AddXP(int amount)
    {
        currentXP += amount;
        UpdateXPText();
    }

    private void UpdateXPText()
    {
        if (xpText != null)
        {
            xpText.text = currentXP.ToString();
        }
    }
}