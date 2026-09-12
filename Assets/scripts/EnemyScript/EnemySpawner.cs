using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configurações de Spawn")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;     // lista de lugares onde ele pode nascer
    public float spawnRate = 2f;        // tempo (em segundos) entre cada inimigo
    
    private float nextSpawnTime = 0f;

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            // Define a hora do proximo spawn
            nextSpawnTime = Time.time + spawnRate; 
        }
    }

    void SpawnEnemy()
    {
        // Escolhe um numero aleatorio entre 0 e a quantidade de pontos de spawn
        int randomIndex = Random.Range(0, spawnPoints.Length);
        
        // Pega o ponto de spawn sorteado
        Transform spawnPoint = spawnPoints[randomIndex];

        // Cria o inimigo naquela posicao e rotacao
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}