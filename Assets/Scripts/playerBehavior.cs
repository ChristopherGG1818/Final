using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class playerBehavior : MonoBehaviour
{
    public float speed = 5f;
    public float sprintMultiplier = 2f;
    public float jumpForce = 10f;

    public LayerMask groundLayer; // Ground layer for trigger check

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public ParticleSystem pixelParticle;
    public GameObject pointLight;
    private bool isAnimated = false;

    [Header("Knockback Settings")]
    public float knockbackDuration = 0.2f;  // How long knockback lasts
    public float knockbackPower = 15f;      // Strength of knockback
    private bool isKnockedback = false;
    private float knockbackTimer = 0f;
    private Vector2 knockbackDirection;

    [Header("Hit Flash Settings")]
    public float flashDuration = 0.25f; // How long each flash lasts
    public int flashCount = 5;          // How many times to flash
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Health")]
    public PlayerHealth health; // Drag your PlayerHealth component here

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    void Update()
    {
        // --- Handle knockback ---
        if (isKnockedback)
        {
            rb.velocity = knockbackDirection;

            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
                isKnockedback = false;

            return; // Skip normal movement while knocked back
        }

        float moveInput = 0f;
        float currentSpeed = speed;

        // Sprint
        if (Keyboard.current.shiftKey.isPressed && isGrounded)
            currentSpeed *= sprintMultiplier;

        // Movement
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            moveInput = -currentSpeed;
        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            moveInput = currentSpeed;

        rb.velocity = new Vector2(moveInput, rb.velocity.y);

        // Jump
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // Better jump physics
        if (rb.velocity.y < 0)
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        else if (rb.velocity.y > 0 && !Keyboard.current.spaceKey.isPressed)
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;

        // Pixel sprint animation
        if (Keyboard.current.shiftKey.isPressed)
        {
            if (!isAnimated)
            {
                pixelParticle.Play();
                if (pointLight != null) pointLight.SetActive(true);
                isAnimated = true;
            }
        }
        else
        {
            if (isAnimated)
            {
                pixelParticle.Stop();
                if (pointLight != null) pointLight.SetActive(false);
                isAnimated = false;
            }
        }

        // Animator
        if (anim != null)
        {
            bool isWalking = Mathf.Abs(moveInput) > 0;
            bool isRunning = Keyboard.current.shiftKey.isPressed && isWalking;

            anim.SetBool("isRunning", isRunning);
            anim.SetBool("isWalking", isWalking);
            anim.SetBool("isGrounded", isGrounded);
            anim.SetFloat("yVelocity", rb.velocity.y);

            // Flip sprite
            if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
            else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    // Trigger-based ground check
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
            isGrounded = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
            isGrounded = false;
    }

    // --- Knockback function ---
    public void ApplyKnockback(Vector2 sourcePosition)
    {
        // Knockback direction
        knockbackDirection = ((Vector2)transform.position - sourcePosition).normalized;
        knockbackDirection *= knockbackPower;
        knockbackDirection.y = knockbackPower / 2f;

        isKnockedback = true;
        knockbackTimer = knockbackDuration;

        // Trigger hit flash
        if (spriteRenderer != null)
        {
            StopAllCoroutines(); // Stop any ongoing flash
            StartCoroutine(FlashCoroutine());
        }

        // Take damage
        if (health != null)
        {
            health.TakeDamage(1); // remove 1 heart per hit
        }
    }

    // --- Hit flash coroutine ---
    private IEnumerator FlashCoroutine()
    {
        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = Color.black;  // Flash color (black)
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor; // Revert to original
            yield return new WaitForSeconds(flashDuration);
        }
    }
}