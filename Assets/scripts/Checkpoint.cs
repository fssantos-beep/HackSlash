using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && CheckpointManager.Instance != null)
        {
            CheckpointManager.Instance.SetCheckpoint(transform.position);
        }
    }
}