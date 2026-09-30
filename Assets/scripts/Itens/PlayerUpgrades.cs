// Aplica upgrades escolhidos e reseta quando  o jogador reinicia o jogo fecha ou volta para o menu
using System.Collections.Generic;

public static class PlayerUpgrades
{
    public struct UpgradeEntry
    {
        public ItemData.EffectType effectType;
        public float value;
    }

    public static List<UpgradeEntry> collected = new List<UpgradeEntry>();

    public static void AddUpgrade(ItemData.EffectType effectType, float value)
    {
        collected.Add(new UpgradeEntry { effectType = effectType, value = value });
    }

    public static void ApplyAllTo(IUpgradable target)
    {
        foreach (var upgrade in collected)
        {
            target.ApplyUpgrade(upgrade.effectType, upgrade.value);
        }
    }

    public static void ResetAll()
    {
        collected.Clear();
    }
}