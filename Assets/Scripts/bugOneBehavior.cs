using UnityEngine;
using System.Collections;

public class bugOneBehavior : MonoBehaviour, IDamageable
{
    [Header("Movement")]
    public float speed = 2f;
    public float minMoveTime = 1f;
    public float maxMoveTime = 3f;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootCooldown = 2f;
    private float shootTimer;

    [Header("Boundaries")]
    public float minX, maxX, minY, maxY;

    [Header("Player")]
    public Transform player;
    public bool playerInRange;

    [Header("Health")]
    public int health = 3;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Color originalColor;

    private Vector2 moveDirection;
    private float moveTimer;

    private bool isKnockedBack;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (sr != null)
            originalColor = sr.color;

        ChooseNewDirection();
        shootTimer = shootCooldown;
    }

    void Update()
    {
        if (playerInRange && player != null)
        {
            FollowPlayer();
            HandleShooting();
        }
        else
        {
            if (!isKnockedBack)
                Move();
        }

        ClampPosition();
    }

    // ---------------- MOVEMENT ----------------
    void Move()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);

        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0f)
            ChooseNewDirection();
    }

    void ChooseNewDirection()
    {
        moveDirection = Random.insideUnitCircle.normalized;
        moveTimer = Random.Range(minMoveTime, maxMoveTime);
    }

    void FollowPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    // ---------------- SHOOT ----------------
    void HandleShooting()
    {
        shootTimer -= Time.deltaTime;

        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootCooldown;
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || player == null) return;

        Vector2 dir = (player.position - firePoint.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.GetComponent<bugProjectile>()?.SetDirection(dir);
    }

    // ---------------- DAMAGE ----------------
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Bug1 hit: " + health);

        if (sr != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashRed());
        }

        if (rb != null && player != null)
        {
            isKnockedBack = true;

            Vector2 dir = (transform.position - player.position).normalized;
            rb.velocity = dir * knockbackForce;

            StartCoroutine(StopKnockback());
        }

        if (health <= 0)
            Destroy(gameObject);
    }

    IEnumerator StopKnockback()
    {
        yield return new WaitForSeconds(knockbackDuration);

        rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }

    IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = originalColor;
    }

    // ---------------- TRIGGERS ----------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    void ClampPosition()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }
}