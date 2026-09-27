// Gerencia os botões na tela de derrota, permitindo que o jogador tente novamente ou volte ao menu principal
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

        if (PersistentUIManager.Instance != null)
        {
            Destroy(PersistentUIManager.Instance.gameObject);
            PersistentUIManager.Instance = null;
        }

        if (ItemSelectionUI.Instance != null)
        {
            Destroy(ItemSelectionUI.Instance.gameObject);
            ItemSelectionUI.Instance = null;
        }

        if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject);
            GameManager.Instance = null;
        }

        PlayerUpgrades.ResetAll();
        SceneManager.LoadScene("Menu");
    }
}