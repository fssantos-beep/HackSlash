using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ItemSelectionUI : MonoBehaviour
{
    public static ItemSelectionUI Instance;

    [Header("Itens disponíveis")]
    public List<ItemData> allItems;

    [Header("Referências UI (3 slots, na mesma ordem)")]
    public GameObject choicePanel;
    public Image[] itemIcons = new Image[3];
    public TMP_Text[] itemNames = new TMP_Text[3];
    public TMP_Text[] itemDescriptions = new TMP_Text[3];
    public Button[] itemButtons = new Button[3];

    private ItemData[] currentChoices = new ItemData[3];

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (choicePanel != null)
            choicePanel.SetActive(false);
    }

    // Chamado pelo PlayerHUD toda vez que o jogador sobe de nível
    public void ShowItemChoices()
    {
        if (allItems == null || allItems.Count == 0)
        {
            Debug.LogWarning("ItemSelectionUI: nenhum item cadastrado em allItems!");
            return;
        }

        // Mesmo valor salvo pelo CharacterSelector e lido pelo CharacterSpawner:
        // 0 = Valina, 1 = Doro
        ItemData.CharacterType activeCharacter = (ItemData.CharacterType)PlayerPrefs.GetInt("SelectedCharacter", 0);

        List<ItemData> pool = new List<ItemData>(allItems);
        List<ItemData> selected = new List<ItemData>();

        // Garante que pelo menos 1 das 3 opções realmente funcione pro personagem atual
        // (item próprio dele ou Universal)
        List<ItemData> usableForActive = pool.FindAll(i =>
            i.usableBy == activeCharacter || i.usableBy == ItemData.CharacterType.Universal);

        if (usableForActive.Count > 0)
        {
            ItemData guaranteed = usableForActive[Random.Range(0, usableForActive.Count)];
            selected.Add(guaranteed);
            pool.Remove(guaranteed);
        }

        // Preenche o resto dos slots livremente, podendo incluir itens do outro personagem
        int remainingSlots = Mathf.Min(3, selected.Count + pool.Count) - selected.Count;
        for (int i = 0; i < remainingSlots; i++)
        {
            int randomIndex = Random.Range(0, pool.Count);
            selected.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex);
        }

        // Embaralha a ordem final pra o item garantido não cair sempre no mesmo slot
        for (int i = 0; i < selected.Count; i++)
        {
            int swapIndex = Random.Range(i, selected.Count);
            ItemData temp = selected[i];
            selected[i] = selected[swapIndex];
            selected[swapIndex] = temp;
        }

        for (int i = 0; i < selected.Count; i++)
        {
            currentChoices[i] = selected[i];
            PopulateSlot(i, selected[i]);
        }

        choicePanel.SetActive(true);
        Time.timeScale = 0f; // pausa o jogo enquanto a pessoa escolhe
    }

    private void PopulateSlot(int index, ItemData item)
    {
        if (itemIcons[index] != null)
            itemIcons[index].sprite = item.icon;

        if (itemNames[index] != null)
            itemNames[index].text = item.itemName;

        if (itemDescriptions[index] != null)
            itemDescriptions[index].text = item.description;

        if (itemButtons[index] != null)
        {
            // Remove listeners antigos antes de adicionar, pra não empilhar
            // callbacks de escolhas anteriores no mesmo botão
            itemButtons[index].onClick.RemoveAllListeners();
            int capturedIndex = index; // evita bug de closure com o índice do loop
            itemButtons[index].onClick.AddListener(() => SelectItem(capturedIndex));
        }
    }

    private void SelectItem(int index)
    {
        ItemData chosen = currentChoices[index];
        ApplyItemToPlayer(chosen);

        choicePanel.SetActive(false);
        Time.timeScale = 1f; // devolve o jogo pro normal
    }

    private void ApplyItemToPlayer(ItemData item)
    {
        ItemData.CharacterType activeCharacter = (ItemData.CharacterType)PlayerPrefs.GetInt("SelectedCharacter", 0);

        // Item exclusivo do outro personagem: a escolha é "gasta", mas não faz nada
        if (item.usableBy != ItemData.CharacterType.Universal && item.usableBy != activeCharacter)
        {
            Debug.Log($"{item.itemName} é exclusivo de outro personagem e não teve efeito.");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("ItemSelectionUI: nenhum GameObject com tag 'Player' encontrado na cena!");
            return;
        }

        IUpgradable upgradable = player.GetComponent<IUpgradable>();
        if (upgradable != null)
        {
            upgradable.ApplyUpgrade(item.effectType, item.effectValue);
        }
        else
        {
            Debug.LogError("ItemSelectionUI: o personagem ativo não implementa IUpgradable!");
        }
    }
}