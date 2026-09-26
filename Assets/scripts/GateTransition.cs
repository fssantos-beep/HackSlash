using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GateTransition : MonoBehaviour
{
    [Header("Destino")]
    public string targetSceneName;
    public string targetEntryId;     // precisa bater com o entryId da outra porta/entrada na cena de destino

    [Header("Interação")]
    public float interactionRange = 1.5f;
    public GameObject interactPrompt;

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

        if (interactPrompt != null)
            interactPrompt.SetActive(inRange);

        if (inRange && Input.GetKeyDown(KeyCode.F))
        {
            TransitionManager.targetEntryId = targetEntryId;
            SceneManager.LoadScene(targetSceneName);
        }
    }
}