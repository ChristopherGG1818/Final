using UnityEngine;
using UnityEngine.InputSystem;

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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
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
}