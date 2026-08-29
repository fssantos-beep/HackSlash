using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Visual")]
    public Transform visuals;

    [Header("Movimento")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator anim;
    private float moveInput;
    private bool facingRight = true;

    [Header("Ataque")]
    public Transform attackPoint; // O pontinho vazio na ponta da espada
    public float attackRange = 0.5f; // O tamanho do alcance da espada
    public LayerMask enemyLayers; // Apenas a layer "Enemy"
    public int attackDamage = 10;
    public float attackCooldown = 0.5f; // Tempo mínimo entre ataques
    private float nextAttackTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = visuals.GetComponent<Animator>();
    }

    void Update()
    {
        // 1. Captura o movimento (A/D ou Setas)
        moveInput = Input.GetAxisRaw("Horizontal");

        // 2. Avisa o Animator para tocar a animação de Run ou Idle
        anim.SetFloat("Speed", Mathf.Abs(moveInput));

        // 3. Vira o personagem
        if (moveInput > 0 && !facingRight) Flip();
        else if (moveInput < 0 && facingRight) Flip();

        // 4. Lógica de Ataque (Botão esquerdo do Mouse ou tecla J)
        // O Time.time >= nextAttackTime impede o spam de ataque
        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.J)) && Time.time >= nextAttackTime)
        {
            Debug.Log("Botão pressionado");
            Attack();
            nextAttackTime = Time.time + 1f / attackCooldown; // Define o cooldown
        }
    }

    void FixedUpdate()
    {
        // Aplica a velocidade física
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void Flip()
    {
        facingRight = !facingRight;
        // Vira o personagem no eixo X
        Vector3 localScale = visuals.localScale;
        localScale.x *= -1;
        visuals.localScale = localScale;
    }

    void Attack()
    {
        // Impede ataque se ainda está no cooldown
        if (Time.time < nextAttackTime) return;
        nextAttackTime = Time.time + attackCooldown;
        Debug.Log("ATAQUE DISPARADO");
        anim.SetTrigger("Attack");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
    {
        enemy.GetComponent<EnemyHealth>()?.TakeDamage(attackDamage);
    }
}

    // Desenha o círculo de ataque no Editor (para você ajustar o alcance)
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}