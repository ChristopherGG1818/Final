using UnityEngine;
using System.Collections;

public class FinalBoss2 : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public int maxHealth = 20;
    private int currentHealth;

    [Header("Player")]
    public Transform player;

    [Header("Movement")]
    public float phase1Speed = 1.5f;
    public float phase2Speed = 3.5f;

    private float currentSpeed;

    [Header("Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    public float phase1FireRate = 2.5f;
    public float phase2FireRate = 1f;

    private float attackTimer;

    [Header("Phase")]
    private bool phase2 = false;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Color originalColor;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;
        currentSpeed = phase1Speed;

        if (sr != null)
            originalColor = sr.color;


            CameraFollow cam = Camera.main.GetComponent<CameraFollow>();

    if (cam != null)
    {
        cam.bossFight = true;
    }
    }

    void Update()
    {
        if (player == null) return;

        HandlePhase();
        FloatTowardPlayer();
        HandleAttacks();
    }

    // PHASE TRANSITION (50%)
    void HandlePhase()
    {
        if (!phase2 && currentHealth <= maxHealth / 2)
        {
            phase2 = true;
            currentSpeed = phase2Speed;
        }
    }

    //RADIANCE-STYLE FLOATING CHASE
    void FloatTowardPlayer()
    {
        Vector2 target = player.position;
        Vector2 dir = (target - (Vector2)transform.position).normalized;

        rb.velocity = dir * currentSpeed;
    }

    //ATTACK SYSTEM
    void HandleAttacks()
    {
        attackTimer += Time.deltaTime;

        float rate = phase2 ? phase2FireRate : phase1FireRate;

        if (attackTimer >= rate)
        {
            Shoot();
            attackTimer = 0f;
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Vector2 dir = (player.position - firePoint.position).normalized;

        float speed = phase2 ? 8f : 4f;

        proj.GetComponent<Rigidbody2D>().velocity = dir * speed;
    }

    // DAMAGE
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        StartCoroutine(Flash());

        if (currentHealth <= 0)
            Die();
    }

    IEnumerator Flash()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = originalColor;
    }

    void Die()
    {
        Destroy(gameObject);
        Debug.Log("Final Boss Defeated!");
    }
}