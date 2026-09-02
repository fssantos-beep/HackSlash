using UnityEngine;

public class AnimationReceiver : MonoBehaviour
{
    private PlayerController playerController;

    void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }
    }

    // Trigga dano na hora do impacto da animacao
    public void TriggerDamage()
    {
        if (playerController != null)
        {
            playerController.OnAttackImpact();
        }
    }
}