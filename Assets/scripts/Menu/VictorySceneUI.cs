// Gerencia a cena de Game Over, permitindo que o jogador reinicie o jogo ou saia
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictorySceneUI : MonoBehaviour
{
    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.victoryClip);
        }
    }

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