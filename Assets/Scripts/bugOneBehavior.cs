using UnityEngine;
using System.Collections;

public class bugOneBehavior : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public float minMoveTime = 1f;
    public float maxMoveTime = 3f;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootCooldown = 2f;
    private float shootTimer;

    [Header("Movement Boundaries")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    [Header("Player Detection")]
    public Transform player;
    public float followRange = 5f;
    public float chaseSpeedMultiplier = 1.5f;

    [Header("Health")]
    public int health = 3;

    //Hit Effects
    [Header("Hit Effects")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private Vector2 moveDirection;
    private float moveTimer;
    private bool playerInRange = false;

    private void Start()
    {
        ChooseNewDirection();

        
        rb = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (playerInRange && player != null)
        {
            FollowPlayerSmooth();
            shootTimer -= Time.deltaTime;

            if (shootTimer <= 0f)
            {
                ShootAtPlayer();
                shootTimer = shootCooldown;
            }
        }
        else
        {
            MoveBug();
            HandleTimer();
        }

        ClampPosition();
    }

    // Random wandering movement
    void MoveBug()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);
        RotateTowardsDirection(moveDirection);
    }

    void HandleTimer()
    {
        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0f)
        {
            ChooseNewDirection();
        }
    }

    //UPDATED TAKE DAMAGE
    public void TakeDamage(int damage)
    {
        health -= damage;

        Debug.Log("Bug hit! Health: " + health);

        //Flash red
        if (spriteRenderer != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashRed());
        }

        //Knockback
        if (rb != null && player != null)
        {
            Vector2 direction = (transform.position - player.position).normalized;
            rb.velocity = direction * knockbackForce;
            StartCoroutine(StopKnockback());
        }

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void ChooseNewDirection()
    {
        moveDirection = Random.insideUnitCircle.normalized;
        moveTimer = Random.Range(minMoveTime, maxMoveTime);
    }

    void FollowPlayerSmooth()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        float step = speed * chaseSpeedMultiplier * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, player.position, step);
    }

    void RotateTowardsDirection(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void ClampPosition()
    {
        Vector3 pos = transform.position;
        bool hitBoundary = false;

        if (pos.x < minX || pos.x > maxX) hitBoundary = true;
        if (pos.y < minY || pos.y > maxY) hitBoundary = true;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;

        if (hitBoundary) ChooseNewDirection();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ChooseNewDirection();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, followRange);
    }

    void ShootAtPlayer()
    {
        if (projectilePrefab == null || player == null) return;

        Vector2 direction = (player.position - firePoint.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        bugProjectile projectileScript = proj.GetComponent<bugProjectile>();
        projectileScript.SetDirection(direction);
    }

    //Flash red
    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    // Stop knockback
    IEnumerator StopKnockback()
    {
        yield return new WaitForSeconds(knockbackDuration);

        if (rb != null)
            rb.velocity = Vector2.zero;
    }
}