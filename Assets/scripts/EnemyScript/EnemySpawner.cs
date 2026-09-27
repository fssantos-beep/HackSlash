using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configurações de Spawn")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;     // lista de lugares onde ele pode nascer
    public float respawnCooldown = 5f; // tempo mínimo entre a morte e o próximo nascer naquele mesmo ponto

    private GameObject[] currentEnemy;
    private float[] nextSpawnTimePerPoint;

    void Start()
    {
        // Inicializa os arrays para controlar os inimigos atuais e o tempo de respawn por ponto
        currentEnemy = new GameObject[spawnPoints.Length];
        nextSpawnTimePerPoint = new float[spawnPoints.Length];

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            SpawnAt(i);
        }
    }

    void Update()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            // Se o inimigo atual naquele ponto morreu e o tempo de respawn já passou, spawn um novo inimigo
            if (currentEnemy[i] == null && Time.time >= nextSpawnTimePerPoint[i])
            {
                SpawnAt(i);
            }
        }
    }

    void SpawnAt(int index)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoints[index].position, spawnPoints[index].rotation);
        currentEnemy[index] = enemy;
        nextSpawnTimePerPoint[index] = Time.time + respawnCooldown;
    }
}