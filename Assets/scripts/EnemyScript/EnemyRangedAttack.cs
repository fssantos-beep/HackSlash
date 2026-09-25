using UnityEngine;
using System.Collections;

public class EnemyRangedAttack : MonoBehaviour
{
    public GameObject projetilPrefab;
    public Transform firePoint;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void FireProjectile()
    {
        if (projetilPrefab == null || firePoint == null) return;

        GameObject proj = Instantiate(projetilPrefab, firePoint.position, Quaternion.identity);

        // Dispara pro lado que o Necromancer está olhando
        float direction = (spriteRenderer != null && spriteRenderer.flipX) ? -1f : 1f;
        proj.GetComponent<Projetil>().SetDirection(new Vector2(direction, 0f));
    }
}