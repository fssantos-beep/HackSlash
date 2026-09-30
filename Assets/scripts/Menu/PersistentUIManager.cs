// Sistema de UI persistente entre cenas, para manter o HUD do jogador e outros elementos de interface
using UnityEngine;

public class PersistentUIManager : MonoBehaviour
{
    public static PersistentUIManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}