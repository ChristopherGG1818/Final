using UnityEngine;
using UnityEngine.InputSystem;

public class playerBehavior : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Animator anim; // Reference to the Animator

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

        // Tell Animator whether we are walking
        if (anim != null)
        {
            bool isWalking = Mathf.Abs(moveInput) > 0;
            anim.SetBool("isWalking", isWalking);

            // Flip sprite depending on direction
            if (moveInput > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (moveInput < 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}