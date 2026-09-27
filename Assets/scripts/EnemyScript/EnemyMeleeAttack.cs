// Gerencia o ataque corpo-a-corpo do inimigo, detectando colisões com o jogador e aplicando dano
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    [Header("Ataque corpo-a-corpo (Attack 1)")]
    public Transform attackPoint;
    public float attackRange = 0.8f;
    public int damage = 10;
    public LayerMask playerLayer;

    // Chamado via Animation Event no frame exato do golpe da espada
    public void OnMeleeImpact()
    {
        if (attackPoint == null) return;

        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);
        if (hit != null)
        {
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}