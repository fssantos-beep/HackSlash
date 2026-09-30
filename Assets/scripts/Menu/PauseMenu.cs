// Script responsável por pausar o jogo e exibir o painel de pausa.
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    public GameObject pausePanel;

    private bool isPaused = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // precisa apertar ESC para pausar o jogo
        {
            if (isPaused)
            {
                Resume();
            }
            else if (Time.timeScale == 1f)
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        isPaused = true;
        if (pausePanel != null) pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        isPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;

        PlayerUpgrades.ResetAll();
        GameProgress.ResetProgress();

        if (ItemSelectionUI.Instance != null) { Destroy(ItemSelectionUI.Instance.gameObject); ItemSelectionUI.Instance = null; }
        if (GameManager.Instance != null) { Destroy(GameManager.Instance.gameObject); GameManager.Instance = null; }

        Destroy(gameObject);
        PersistentUIManager.Instance = null;

        SceneManager.LoadScene("Menu");
    }
}