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

    [Header("Cycle Timing")]
    public float chaseTime = 5f;
    public float attackTime = 2f;

    private bool isChasing = true;

    [Header("Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    public float phase1FireRate = 0.5f;
    public float phase2FireRate = 0.2f;

    private bool phase2 = false;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Color originalColor;

    // Spiral settings
    [Header("Spiral Attack")]
    public int spiralBullets = 12;
    public float spiralSpeed = 6f;
    public float spiralAngleStep = 25f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;
        currentSpeed = phase1Speed;

        rb.gravityScale = 0;
        rb.freezeRotation = true;

        if (sr != null)
            originalColor = sr.color;

        // Camera boss mode
        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        if (cam != null)
            cam.bossFight = true;

        // Stop music
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            MusicLayers music = playerObj.GetComponentInChildren<MusicLayers>();

            if (music != null)
                music.StopAllMusic();
        }

        StartCoroutine(BossCycle());
    }

    void Update()
    {
        if (player == null) return;

        if (isChasing)
            ChasePlayer();
    }

    //MAIN LOOP
    IEnumerator BossCycle()
    {
        while (true)
        {
            // CHASE
            isChasing = true;
            yield return new WaitForSeconds(chaseTime);

            // ATTACK
            isChasing = false;
            rb.velocity = Vector2.zero;

            yield return StartCoroutine(AttackBurst());

            yield return new WaitForSeconds(attackTime);
        }
    }

    // CHASE MOVEMENT
    void ChasePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.velocity = Vector2.Lerp(rb.velocity, dir * currentSpeed, 0.1f);
    }

    //ATTACK BURST (structured pattern)
    IEnumerator AttackBurst()
    {
        int shots = phase2 ? 14 : 8;

        for (int i = 0; i < shots; i++)
        {
            ShootPattern();
            yield return new WaitForSeconds(phase2FireRate);
        }
    }

    //ATTACK ROTATION SYSTEM (NO MORE PURE RANDOM CHAOS)
    void ShootPattern()
    {
        if (projectilePrefab == null || firePoint == null) return;

        int pattern = Random.Range(0, 3);

        if (pattern == 0)
            ShootAtPlayer();
        else if (pattern == 1)
            ShootRandom();
        else
            ShootSpiral();
    }

    // DIRECT SHOT
    void ShootAtPlayer()
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Vector2 dir = (player.position - firePoint.position).normalized;

        float speed = phase2 ? 14f : 8f;

        proj.GetComponent<Rigidbody2D>().velocity = dir * speed;
    }

    // RANDOM SHOT
    void ShootRandom()
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Vector2 dir = new Vector2(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;

        float speed = phase2 ? 10f : 6f;

        proj.GetComponent<Rigidbody2D>().velocity = dir * speed;
    }

    // SPIRAL ATTACK
    void ShootSpiral()
    {
        float angleOffset = Time.time * (phase2 ? 300f : 180f);

        for (int i = 0; i < spiralBullets; i++)
        {
            float angle = angleOffset + (i * spiralAngleStep);
            float rad = angle * Mathf.Deg2Rad;

            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            float speed = phase2 ? 10f : spiralSpeed;

            proj.GetComponent<Rigidbody2D>().velocity = dir * speed;
        }
    }

    // DAMAGE SYSTEM
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        StartCoroutine(Flash());

        if (!phase2 && currentHealth <= maxHealth / 2)
        {
            phase2 = true;
            currentSpeed = phase2Speed;

            if (sr != null)
                sr.color = Color.yellow;
        }

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