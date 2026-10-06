using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configurações de Spawn")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;     // lista de lugares onde ele pode nascer
    public float respawnCooldown = 5f; // tempo mínimo entre a morte e o próximo nascer naquele mesmo ponto

    private GameObject[] currentEnemy;
    private float[] nextSpawnTimePerPoint;
    private bool[] waitingToRespawn; // controla se já começou a contar o cooldown desse ponto

    void Start()
    {
        currentEnemy = new GameObject[spawnPoints.Length];
        nextSpawnTimePerPoint = new float[spawnPoints.Length];
        waitingToRespawn = new bool[spawnPoints.Length];

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            SpawnAt(i);
        }
    }

    void Update()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (currentEnemy[i] == null)
            {
                if (!waitingToRespawn[i])
                {
                    // O inimigo morreu, então começa a contar o cooldown para respawn
                    waitingToRespawn[i] = true;
                    nextSpawnTimePerPoint[i] = Time.time + respawnCooldown;
                }
                else if (Time.time >= nextSpawnTimePerPoint[i])
                {
                    SpawnAt(i);
                    waitingToRespawn[i] = false;
                }
            }
        }
    }

    void SpawnAt(int index)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoints[index].position, spawnPoints[index].rotation);
        currentEnemy[index] = enemy;
    }
}