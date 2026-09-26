using UnityEngine;

public class SceneEntryPoint : MonoBehaviour
{
    public string entryId; // ex: "PortalArea1ParaArea2" precisa bater com o targetEntryId do portal de origem

    void Start()
    {
        if (!string.IsNullOrEmpty(TransitionManager.targetEntryId) && TransitionManager.targetEntryId == entryId)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                player.transform.position = transform.position;

            TransitionManager.targetEntryId = "";
        }
    }
}