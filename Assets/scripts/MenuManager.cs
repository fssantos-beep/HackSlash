using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Nome da cena de seleção de personagem")]
    public string characterSelectionSceneName = "CharacterSelectionScene";

    public void StartGame()
    {
        SceneManager.LoadScene(characterSelectionSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Fechando o jogo...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // só funciona no .exe buildado, não no Editor
#endif
    }
}