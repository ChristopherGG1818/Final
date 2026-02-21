using UnityEngine;
using UnityEngine.InputSystem;

public class playerBehavior : MonoBehaviour
{
    public float speed = 5f;

    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckDistance= 0.2f;
    public LayerMask groundLayer;


    private Rigidbody2D rb;
    private Animator anim; // Reference to the Animator
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); // Make sure the Animator is on the same GameObject
    }

    void Update()
    {
        float moveInput = 0f;

        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            moveInput = -speed;

        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            moveInput = speed;

        rb.velocity = new Vector2(moveInput, rb.velocity.y);

        //cech for the ground
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

        //jump fix this part 
        // if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded){
        //     rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        // }
        //     //moveInput = speed;

        // Tell Animator whether we are walking
        if (anim != null)
        {
            bool isWalking = Mathf.Abs(moveInput) > 0;
            anim.SetBool("isWalking", isWalking);
            anim.SetBool("isGrounded", isGrounded);

            // Flip sprite depending on direction
            if (moveInput > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (moveInput < 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}