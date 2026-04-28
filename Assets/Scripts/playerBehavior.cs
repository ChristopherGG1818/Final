using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class playerBehavior : MonoBehaviour
{
    public float speed = 5f;
    public float sprintMultiplier = 2f;
    public float jumpForce = 10f;

    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public ParticleSystem pixelParticle;
    private bool isAnimated = false;

    [Header("Knockback Settings")]
    public float knockbackDuration = 0.2f;
    public float knockbackPower = 15f;
    private bool isKnockedback = false;
    private float knockbackTimer = 0f;
    private Vector2 knockbackDirection;

    [Header("Hit Flash Settings")]
    public float flashDuration = 0.25f;
    public int flashCount = 5;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Health")]
    public PlayerHealth health;

    [Header("Footsteps")]
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;
    public float stepRate = 0.35f;
    private float stepTimer;

    public Sword sword;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        //LOAD SAVED HEALTH FROM GAMEMANAGER
        if (health != null && GameManager.instance != null)
        {
            health.CurrentHealth = GameManager.instance.health;
        }
    }

    void Update()
    {
        if (isKnockedback)
        {
            rb.velocity = knockbackDirection;

            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
                isKnockedback = false;

            return;
        }

        float moveInput = 0f;
        float currentSpeed = speed;

        if (Keyboard.current.shiftKey.isPressed && isGrounded)
            currentSpeed *= sprintMultiplier;

        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            moveInput = -currentSpeed;
        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            moveInput = currentSpeed;

        rb.velocity = new Vector2(moveInput, rb.velocity.y);

        HandleFootsteps(moveInput);

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (rb.velocity.y < 0)
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        else if (rb.velocity.y > 0 && !Keyboard.current.spaceKey.isPressed)
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;

        if (Keyboard.current.shiftKey.isPressed)
        {
            if (!isAnimated)
            {
                if (pixelParticle != null)
                    pixelParticle.Play();

                isAnimated = true;
            }
        }
        else
        {
            if (isAnimated)
            {
                if (pixelParticle != null)
                    pixelParticle.Stop();

                isAnimated = false;
            }
        }

        if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            if (sword != null)
                sword.Attack();
        }

        if (anim != null)
        {
            bool isWalking = Mathf.Abs(moveInput) > 0;
            bool isRunning = Keyboard.current.shiftKey.isPressed && isWalking;

            anim.SetBool("isRunning", isRunning);
            anim.SetBool("isWalking", isWalking);
            anim.SetBool("isGrounded", isGrounded);
            anim.SetFloat("yVelocity", rb.velocity.y);

            if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
            else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void HandleFootsteps(float moveInput)
    {
        bool isMoving = Mathf.Abs(moveInput) > 0.1f && isGrounded;

        if (isMoving)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepRate;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    void PlayFootstep()
    {
        if (footstepClips.Length == 0 || footstepSource == null) return;

        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        footstepSource.PlayOneShot(clip);
    }

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

    public void ApplyKnockback(Vector2 sourcePosition)
    {
        knockbackDirection = ((Vector2)transform.position - sourcePosition).normalized;
        knockbackDirection *= knockbackPower;
        knockbackDirection.y = knockbackPower / 2f;

        isKnockedback = true;
        knockbackTimer = knockbackDuration;

        if (spriteRenderer != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashCoroutine());
        }

        if (health != null)
        {
            health.TakeDamage(1);

            //SAVE UPDATED HEALTH
            if (GameManager.instance != null)
            {
                GameManager.instance.health = health.CurrentHealth;
            }
        }
    }

    private IEnumerator FlashCoroutine()
    {
        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = Color.black;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
    }
}