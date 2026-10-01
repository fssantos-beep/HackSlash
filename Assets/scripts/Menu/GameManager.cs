// Gerencia o estado geral do jogo, incluindo a exibição da tela de derrota e a pausa do jogo
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject defeatScreen;

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
        DontDestroyOnLoad(gameObject);
    }

    public void ShowDefeat()
    {
        if (defeatScreen != null)
        {
            TMP_Text text = defeatScreen.GetComponent<TMP_Text>();
            if (text != null) text.text = "Derrota!";
            defeatScreen.SetActive(true);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.gameOverClip);
        }
        Time.timeScale = 0f;
    }
}