// Gerencia a transição entre cenas quando o jogador interage com um portão, incluindo prompts de interação e condições de desbloqueio
using UnityEngine;
using UnityEngine.SceneManagement;

public class GateTransition : MonoBehaviour
{
    [Header("Destino")]
    public string targetSceneName; // nome da cena para qual o portão leva
    public string targetEntryId; // precisa bater com o entryId do outro objeto de entrada na cena

    [Header("Interação")]
    public float interactionRange = 1.5f;
    public GameObject interactPrompt;

    [Header("Condição de desbloqueio")]
    public bool requiresBossDefeat = false;
    public GameObject lockedPrompt; // um texto pedindo para derrotar o boss antes de finalizar o jogo
    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        bool inRange = distance <= interactionRange;
        bool unlocked = !requiresBossDefeat || GameProgress.bossDefeated;

        if (interactPrompt != null)
            interactPrompt.SetActive(inRange && unlocked);

        if (lockedPrompt != null)
            lockedPrompt.SetActive(inRange && !unlocked);

        if (inRange && unlocked && Input.GetKeyDown(KeyCode.F))
        {
            TransitionManager.targetEntryId = targetEntryId;
            SceneManager.LoadScene(targetSceneName);
        }
    }
}