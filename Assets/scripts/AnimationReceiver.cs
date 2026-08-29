using UnityEngine;

public class AnimationReceiver : MonoBehaviour
{
    public PlayerController PlayerController;

    public void applyDamage()
    {
        if (PlayerController != null)
        {
            PlayerController.applyDamage(); 
        }
    }
}