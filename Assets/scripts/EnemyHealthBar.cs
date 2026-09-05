using UnityEngine;
using UnityEngine.UI;

// Controla a barra de vida visual que fica em cima do inimigo.
public class EnemyHealthBar : MonoBehaviour
{
    public Slider slider;
    public EnemyHealth enemyHealth;

    void Start()
    {
        // Se não arrastou no inspector, tenta achar automaticamente no pai
        if (enemyHealth == null) 
            enemyHealth = GetComponentInParent<EnemyHealth>();
            
        if (slider != null && enemyHealth != null)
        {
            slider.maxValue = enemyHealth.maxHealth;
            slider.value = enemyHealth.currentHealth;
        }
    }

    // Chamado pelo EnemyHealth quando toma dano
    public void UpdateBar()
    {
        if (slider != null && enemyHealth != null)
        {
            slider.value = enemyHealth.currentHealth;
        }
    }
}