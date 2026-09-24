using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject defeatScreen;

    void Awake()
    {
        Instance = this;
    }

    public void ShowDefeat()
    {
        if (defeatScreen != null)
        {
            TMP_Text text = defeatScreen.GetComponent<TMP_Text>();
            if (text != null) text.text = "Derrota!";
            defeatScreen.SetActive(true);
        }
        Time.timeScale = 0f;
    }
}