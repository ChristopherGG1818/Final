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

    //faster attack rhythm
    public float phase1FireRate = 0.3f;
    public float phase2FireRate = 0.2f;

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

        rb.gravityScale = 0;
        rb.freezeRotation = true;

        if (sr != null)
            originalColor = sr.color;

        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        if (cam != null)
            cam.bossFight = true;

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

    IEnumerator BossCycle()
    {
        while (true)
        {
            isChasing = true;
            yield return new WaitForSeconds(chaseTime);

            isChasing = false;
            rb.velocity = Vector2.zero;

            yield return StartCoroutine(AttackBurst());

            yield return new WaitForSeconds(attackTime);
        }
    }

    void ChasePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;

        rb.velocity = Vector2.Lerp(rb.velocity, dir * currentSpeed, 0.1f);
    }

    // MORE BULLETS + FASTER BURST
    IEnumerator AttackBurst()
    {
        int shots = phase2 ? 20 : 12; // increased from  phase 1

        for (int i = 0; i < shots; i++)
        {
            ShootPattern();
            yield return new WaitForSeconds(phase2FireRate);
        }
    }

    void ShootPattern()
    {
        if (projectilePrefab == null || firePoint == null) return;

        if (Random.Range(0, 2) == 0)
            ShootAtPlayer();
        else
            ShootRandom();
    }

    void ShootAtPlayer()
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Vector2 dir = (player.position - firePoint.position).normalized;

        float speed = phase2 ? 14f : 8f; // faster bullets

        proj.GetComponent<Rigidbody2D>().velocity = dir * speed;
    }

    void ShootRandom()
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Vector2 dir = new Vector2(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;

        float speed = phase2 ? 10f : 6f; //faster chaos shots

        proj.GetComponent<Rigidbody2D>().velocity = dir * speed;
    }

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