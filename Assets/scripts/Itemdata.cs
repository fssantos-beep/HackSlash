using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Itens/Item")]
public class ItemData : ScriptableObject
{
    public enum EffectType { AttackDamage, MoveSpeed, MaxHealth, AttackRange }
    public enum CharacterType { Valina, Doro, Universal }

    [Header("Aparência")]
    public string itemName;
    [TextArea] public string description;
    public Sprite icon; // recortado da sprite sheet de itens

    [Header("Efeito")]
    public EffectType effectType;
    public float effectValue;

    [Header("Restrição de personagem")]
    [Tooltip("Universal = funciona pra Doro e Valina. Escolher um personagem específico faz o item não ter efeito nenhum se aplicado no outro.")]
    public CharacterType usableBy = CharacterType.Universal;
}