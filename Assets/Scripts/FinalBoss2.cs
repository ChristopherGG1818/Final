using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FinalBoss2 : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public int maxHealth = 30;
    private int currentHealth;

    [Header("Player")]
    public Transform player;

    [Header("Movement")]
    public float phase1Speed = 1.5f;
    public float phase2Speed = 3.5f;
    public float phase3Speed = 5f;
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
    private bool phase3 = false;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Color originalColor;

    [Header("Spiral Attack")]
    public int spiralBullets = 12;
    public float spiralSpeed = 6f;
    public float spiralAngleStep = 25f;
    private float spiralAngle = 0f;

    private int patternIndex = 0;

    [Header("Teleport")]
    public float teleportCooldown = 3f;
    public float teleportRange = 6f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip phaseChangeSound;

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

            if (phase3)
                yield return StartCoroutine(Teleport());

            yield return new WaitForSeconds(0.5f);

            yield return StartCoroutine(AttackBurst());

            yield return new WaitForSeconds(attackTime);
        }
    }

    void ChasePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.velocity = Vector2.Lerp(rb.velocity, dir * currentSpeed, 0.1f);
    }

    IEnumerator AttackBurst()
    {
        int shots;
        float fireRate;

        if (phase3)
        {
            shots = 20;
            fireRate = 0.1f;
        }
        else if (phase2)
        {
            shots = 14;
            fireRate = phase2FireRate;
        }
        else
        {
            shots = 8;
            fireRate = phase1FireRate;
        }

        for (int i = 0; i < shots; i++)
        {
            ShootPattern();
            yield return new WaitForSeconds(fireRate);
        }
    }

    void ShootPattern()
    {
        if (projectilePrefab == null || firePoint == null) return;

        if (patternIndex == 0)
            ShootAtPlayer();
        else if (patternIndex == 1)
            ShootRandom();
        else
            ShootSpiral();

        patternIndex = (patternIndex + 1) % 3;
    }

    void ShootAtPlayer()
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Vector2 dir = (player.position - firePoint.position).normalized;
        float speed = phase3 ? 16f : (phase2 ? 14f : 8f);

        proj.GetComponent<Rigidbody2D>().velocity = dir * speed;
    }

    void ShootRandom()
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Vector2 dir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

        float speed = phase3 ? 12f : (phase2 ? 10f : 6f);

        proj.GetComponent<Rigidbody2D>().velocity = dir * speed;
    }

    void ShootSpiral()
    {
        int bullets = phase3 ? spiralBullets + 6 : spiralBullets;

        for (int i = 0; i < bullets; i++)
        {
            float angle = spiralAngle + (i * spiralAngleStep);
            float rad = angle * Mathf.Deg2Rad;

            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            float speed = phase3 ? 12f : (phase2 ? 10f : spiralSpeed);

            proj.GetComponent<Rigidbody2D>().velocity = dir * speed;
        }

        spiralAngle += phase3 ? 25f : (phase2 ? 20f : 10f);
    }

    IEnumerator TeleportLoop()
    {
        while (phase3)
        {
            yield return new WaitForSeconds(teleportCooldown);
            yield return StartCoroutine(Teleport());
        }
    }

    IEnumerator Teleport()
    {
        rb.velocity = Vector2.zero;

        if (sr != null)
            sr.enabled = false;

        yield return new WaitForSeconds(0.3f);

        Vector2 randomOffset = Random.insideUnitCircle * teleportRange;
        transform.position = (Vector2)player.position + randomOffset;

        if (sr != null)
            sr.enabled = true;

        yield return new WaitForSeconds(0.2f);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        StartCoroutine(Flash());

        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();

        // PHASE 2
        if (!phase2 && currentHealth <= maxHealth * 0.5f)
        {
            phase2 = true;
            currentSpeed = phase2Speed;

            if (sr != null)
                sr.color = Color.yellow;

            if (cam != null)
                cam.Shake(0.3f);

            PlayPhaseSound();
        }

        // PHASE 3
        if (!phase3 && currentHealth <= maxHealth * 0.25f)
        {
            phase3 = true;
            currentSpeed = phase3Speed;

            if (sr != null)
                sr.color = Color.magenta;

            if (cam != null)
                cam.Shake(0.5f);

            StartCoroutine(TeleportLoop());

            PlayPhaseSound();
        }

        if (currentHealth <= 0)
            Die();
    }

    void PlayPhaseSound()
    {
        if (audioSource != null && phaseChangeSound != null)
            audioSource.PlayOneShot(phaseChangeSound);
    }

    IEnumerator Flash()
    {
        if (sr == null) yield break;

        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = originalColor;
    }

    void Die()
    {
        Debug.Log("Final Boss Defeated!");

        StopAllCoroutines();
        rb.velocity = Vector2.zero;

        StartCoroutine(LoadWinScene());
    }

    IEnumerator LoadWinScene()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Winner");
    }
}