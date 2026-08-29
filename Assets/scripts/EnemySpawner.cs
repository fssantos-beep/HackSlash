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
            // define a hora do proximo spawn
            nextSpawnTime = Time.time + spawnRate; 
        }
    }

    void SpawnEnemy()
    {
        // escolhe um numero aleatorio entre 0 e a quantidade de pontos de spawn
        int randomIndex = Random.Range(0, spawnPoints.Length);
        
        // pega o ponto de spawn sorteado
        Transform spawnPoint = spawnPoints[randomIndex];

        // cria o inimigo naquela posicao e rotacao
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}