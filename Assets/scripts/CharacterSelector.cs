using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterSelector : MonoBehaviour
{
    [Header("Personagens")]
    public Image valinaImg;
    public Image doroImg;

    [Header("Textos")]
    public TMP_Text Valinatxt;
    public TMP_Text Dorotxt;

    [Header("Botões")]
    public Button ConfirmarButton;
    public Button VoltarButton;

    private int selectedIndex = 0;

    void Start()
    {
        UpdateSelection();
    }

    // Precisam ser public pra aparecer no OnClick() do Inspector
    public void SelectValina()
    {
        selectedIndex = 0;
        UpdateSelection();
    }

    public void SelectDoro()
    {
        selectedIndex = 1;
        UpdateSelection();
    }

    // Atualiza o visual pra deixar claro qual personagem está selecionado no momento
    void UpdateSelection()
    {
        if (valinaImg != null)
            valinaImg.color = (selectedIndex == 0) ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.8f);

        if (Valinatxt != null)
        {
            Valinatxt.color = (selectedIndex == 0) ? Color.white : new Color(0.7f, 0.7f, 0.7f);
            Valinatxt.fontStyle = (selectedIndex == 0) ? FontStyles.Bold : FontStyles.Normal;
        }

        if (doroImg != null)
            doroImg.color = (selectedIndex == 1) ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.8f);

        if (Dorotxt != null)
        {
            Dorotxt.color = (selectedIndex == 1) ? Color.white : new Color(0.7f, 0.7f, 0.7f);
            Dorotxt.fontStyle = (selectedIndex == 1) ? FontStyles.Bold : FontStyles.Normal;
        }
    }

    // Salva a escolha pra o CharacterSpawner ler na próxima cena
    public void ConfirmSelection()
    {
        PlayerPrefs.SetInt("SelectedCharacter", selectedIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Área 1");
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}