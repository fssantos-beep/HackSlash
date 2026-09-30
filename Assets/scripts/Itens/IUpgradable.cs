using UnityEngine;

// Permite que a tela de escolha de item aplique um upgrade sem precisar
// saber se o personagem ativo é a Doro ou a Valina
public interface IUpgradable
{
    void ApplyUpgrade(ItemData.EffectType effectType, float value);
}