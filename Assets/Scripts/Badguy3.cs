using UnityEngine;
using System.Collections;

public class Badguy3 : MonoBehaviour, IDamageable
{
    [Header("Patrol")]
    public float speed = 2f;
    public float minX;
    public float maxX;
    private int direction = 1;

    [Header("Charge")]
    public float detectionRange = 5f;
    public float chargeSpeed = 8f;
    public float chargeDuration = 1f;

    [Header("Player")]
    public Transform player;

    [Header("Damage")]
    public float hitCooldown = 1f;
    private bool canHitPlayer = true;

    [Header("Health")]
    public int health = 3;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Color originalColor;

    private bool isCharging;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (sr != null)
            originalColor = sr.color;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (!isCharging && dist <= detectionRange)
            StartCoroutine(Charge());

        if (!isCharging)
            Patrol();
    }

    void Patrol()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        if (transform.position.x >= maxX)
            direction = -1;

        if (transform.position.x <= minX)
            direction = 1;
    }

    IEnumerator Charge()
    {
        isCharging = true;

        yield return new WaitForSeconds(0.2f); // wind-up

        Vector2 dir = (player.position - transform.position).normalized;

        float t = 0f;

        while (t < chargeDuration)
        {
            rb.velocity = dir * chargeSpeed;
            t += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        isCharging = false;
    }

    // DAMAGE TO ENEMY 
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (sr != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashRed());
        }

        if (health <= 0)
            Destroy(gameObject);
    }

    IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = originalColor;
    }

    // PLAYER DAMAGE
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canHitPlayer) return;

        if (collision.CompareTag("Player"))
        {
            playerBehavior playerScript = collision.GetComponent<playerBehavior>();

            if (playerScript != null)
            {
                // THIS is your real damage flow
                playerScript.ApplyKnockback(transform.position);

                StartCoroutine(HitCooldown());
            }
        }

        // stop charge on hit
        if (isCharging)
        {
            rb.velocity = Vector2.zero;
            isCharging = false;
        }
    }

    IEnumerator HitCooldown()
    {
        canHitPlayer = false;
        yield return new WaitForSeconds(hitCooldown);
        canHitPlayer = true;
    }
}