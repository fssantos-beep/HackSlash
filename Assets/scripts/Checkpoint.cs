// Checkpoint que salva a posição do jogador quando ele entra em contato, permitindo que ele reapareça nesse ponto após morrer
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