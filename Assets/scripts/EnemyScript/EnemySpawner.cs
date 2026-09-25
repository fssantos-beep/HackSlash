using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configurações de Spawn")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;     // lista de lugares onde ele pode nascer
    public float spawnRate = 2f; // tempo entre cada spawn sequencial

    private float nextSpawnTime = 0f;
    private int nextSpawnIndex = 0; // qual ponto vem a seguir em ordem

    void Update()
    {
        // Se tem inimigo em todos os pontos disponíveis não spawna mais nada
        if (nextSpawnIndex >= spawnPoints.Length) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void SpawnEnemy()
    {
        Transform spawnPoint = spawnPoints[nextSpawnIndex];
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        nextSpawnIndex++;
    }
}