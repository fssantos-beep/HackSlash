using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatButton : MonoBehaviour
{
    public GameObject defeatScreen;

    public void OnRetryPressed()
    {
        Time.timeScale = 1f;

        if (defeatScreen != null)
            defeatScreen.SetActive(false);

        if (PlayerHealth.Instance != null)
            PlayerHealth.Instance.ResetAfterDeath();
    }

    public void OnBackToMenuPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}