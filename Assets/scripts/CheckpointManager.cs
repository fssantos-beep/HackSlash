using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;
    private Vector3 currentCheckpoint;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Se o player ainda não tocou em nenhum checkpoint, usa a posição inicial dele
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            currentCheckpoint = player.transform.position;
    }

    public void SetCheckpoint(Vector3 position)
    {
        currentCheckpoint = position;
    }

    public Vector3 GetCheckpoint()
    {
        return currentCheckpoint;
    }
}