using UnityEngine;

public class AnimationReceiver : MonoBehaviour
{
    private IAttackable attackable;

    void Awake()
    {
        // Procura o script no objeto pai primeiro
        attackable = GetComponentInParent<IAttackable>();

        // Se não achou, tenta no próprio objeto
        if (attackable == null)
        {
            attackable = GetComponent<IAttackable>();
        }

        if (attackable == null)
        {
            Debug.LogError("AnimationReceiver: nenhum personagem com IAttackable encontrado!");
        }
    }

    // Trigga dano na hora do impacto da animacao
    public void TriggerDamage()
    {
        if (attackable != null)
        {
            attackable.OnAttackImpact();
        }
    }
}