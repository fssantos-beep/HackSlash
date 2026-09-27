// Gerencia a cena de Game Over, permitindo que o jogador reinicie o jogo ou saia
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverSceneUI : MonoBehaviour
{
    public void PlayAgain()
    {
        Time.timeScale = 1f;

        PlayerUpgrades.ResetAll();
        GameProgress.ResetProgress();

        if (PersistentUIManager.Instance != null) { Destroy(PersistentUIManager.Instance.gameObject); PersistentUIManager.Instance = null; }
        if (ItemSelectionUI.Instance != null) { Destroy(ItemSelectionUI.Instance.gameObject); ItemSelectionUI.Instance = null; }
        if (GameManager.Instance != null) { Destroy(GameManager.Instance.gameObject); GameManager.Instance = null; }

        SceneManager.LoadScene("CharacterSelection");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}