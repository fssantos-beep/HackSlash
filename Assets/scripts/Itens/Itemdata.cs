// ScriptableObject que define os dados de um item, incluindo nome, descrição, ícone, efeito e restrição de personagem
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Itens/Item")]
public class ItemData : ScriptableObject
{
    public enum EffectType
    {
        AttackDamage,            // soma valor fixo no dano do ataque básico
        MoveSpeed,               // soma valor fixo na velocidade
        MaxHealth,               // soma valor fixo na vida máxima
        AttackRange,             // soma valor fixo no alcance
        AttackDamagePercent,     // % de aumento no dano do ataque básico
        AttackSpeedPercent,      // % de redução (positivo = mais rápido) no cooldown do ataque básico
        SpecialCooldownPercent,  // % de redução no cooldown do especial (Doro) / Dash Attack (Valina)
        SpecialAttackDamage,     // soma valor fixo no dano do especial (Doro) / Dash Attack (Valina)
        DamageReductionPercent,  // % de redução no dano recebido
        DodgeChancePercent,      // % de chance de ignorar o dano totalmente
        HealOnKill,              // cura X de vida ao derrotar um inimigo
        MaxHealthMultiplier,     // multiplica a vida máxima atual (2 = dobra)
        AttackDamageMultiplier   // multiplica o dano do ataque básico atual (2 = dobra)
    }

    public enum CharacterType { Valina, Doro, Universal }

    [Header("Aparência")]
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Efeito principal")]
    public EffectType effectType;
    public float effectValue;

    [Header("Efeito secundário (opcional)")]
    [Tooltip("Use para itens que mexem em duas coisas ao mesmo tempo, como a Espada da Valina (dano x2, ataque mais lento)")]
    public bool hasSecondaryEffect = false;
    public EffectType secondaryEffectType;
    public float secondaryEffectValue;

    [Header("Restrição de personagem")]
    [Tooltip("Universal = funciona pra Doro e Valina. Escolher um personagem específico faz o item não ter efeito nenhum se aplicado no outro.")]
    public CharacterType usableBy = CharacterType.Universal;
}