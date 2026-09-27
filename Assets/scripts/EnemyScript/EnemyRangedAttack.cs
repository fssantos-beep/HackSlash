// Gerencia o ataque à distância do inimigo, instanciando projéteis e definindo sua direção com base na direção do inimigo
using UnityEngine;

public class EnemyRangedAttack : MonoBehaviour
{
    [Header("Ataque à distância (Attack 2)")]
    public GameObject projetilPrefab;
    public Transform firePoint;
    private EnemyMovement enemyMovement;

    void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
    }

    // Chamado via Animation Event no frame exato em que solta o projétil
    public void FireProjectile()
    {
        if (projetilPrefab == null || firePoint == null || enemyMovement == null) return;

        GameObject proj = Instantiate(projetilPrefab, firePoint.position, Quaternion.identity);
        proj.GetComponent<Projetil>().SetDirection(new Vector2(enemyMovement.facingDirection, 0f));
    }
}