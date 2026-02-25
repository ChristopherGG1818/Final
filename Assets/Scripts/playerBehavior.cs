using UnityEngine;
using UnityEngine.InputSystem;

public class playerBehavior : MonoBehaviour
{
    public float speed = 5f;
    public float sprintMultiplier = 2f;
    public float jumpForce = 10f;

    public Transform groundCheck;
    public float groundCheckDistance= 0.2f;
    public LayerMask groundLayer;


    private Rigidbody2D rb;
    private Animator anim; // Reference to the Animator
    private bool isGrounded;


    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;


    public ParticleSystem pixelParticle;
    public GameObject pointLight;
    private bool isAnimated = false;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); // Make sure the Animator is on the same GameObject
    }

    void Update()
    {
        float moveInput = 0f;

        float currentSpeed = speed;

        if (Keyboard.current.shiftKey.isPressed  && isGrounded)
        {
            currentSpeed *= sprintMultiplier; // Increase speed while Shift is held
        }

        //i need movemnt 
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed){
            moveInput = -currentSpeed;
        }
        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
        {
            moveInput = currentSpeed;
        }
        rb.velocity = new Vector2(moveInput, rb.velocity.y);

        //cechk for the ground
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);



        //jump fix this part 
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded){
            Debug.Log("Space was pressed!");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        // Better jump physics
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Keyboard.current.spaceKey.isPressed)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        


        //pixel animation
        if (Keyboard.current.shiftKey.isPressed)
        {
            if (!isAnimated)
            {
                pixelParticle.Play();
                if (pointLight != null)
                pointLight.SetActive(true);
                isAnimated = true;
            }
        }
        else
        {
            if (isAnimated)
            {
                pixelParticle.Stop();
                if (pointLight != null)
                pointLight.SetActive(false);
                isAnimated = false;
            }
        }

        // Tell Animator whether what is our action
        if (anim != null)
        {
            bool isWalking = Mathf.Abs(moveInput) > 0;
            bool isRunning = Keyboard.current.shiftKey.isPressed && isWalking;

            anim.SetBool("isRunning", isRunning);
            anim.SetBool("isWalking", isWalking);
            anim.SetBool("isGrounded", isGrounded);
            anim.SetFloat("yVelocity", rb.velocity.y);

            // Flip sprite depending on direction
            if (moveInput > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (moveInput < 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}