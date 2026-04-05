using UnityEngine;

public class bugOneBehavior : MonoBehaviour{
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
    public Transform player;        // Assign your main protagonist in Inspector
    public float followRange = 5f;  // Radius to start following
    public float chaseSpeedMultiplier = 1.5f; // Slightly faster when chasing

    private Vector2 moveDirection;
    private float moveTimer;
    private bool playerInRange = false;

    private void Start()
    {
        ChooseNewDirection();
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

    void ChooseNewDirection()
    {
        moveDirection = Random.insideUnitCircle.normalized;
        moveTimer = Random.Range(minMoveTime, maxMoveTime);
    }

    // Smooth player following and facing
    void FollowPlayerSmooth()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        // Rotate to face the player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Move toward the player
        float step = speed * chaseSpeedMultiplier * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, player.position, step);
    }

    // Rotate bug toward its movement vector for wandering
    void RotateTowardsDirection(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    // Keep bug inside boundaries
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

    // Change direction on collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ChooseNewDirection();
    }

    // Detect player entering/exiting radius
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

    // Draw detection radius in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, followRange);
    }

    //shoot at main protagonist
    void ShootAtPlayer()
    {
    if (projectilePrefab == null || player == null) return;
    
    Vector2 direction = (player.position - firePoint.position).normalized;
    
    GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
    bugProjectile projectileScript = proj.GetComponent<bugProjectile>();
    projectileScript.SetDirection(direction);
    }
}