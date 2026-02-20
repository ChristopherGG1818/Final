using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class box : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;

    private Rigidbody2D rb;

    void Awake()
    {
        // Get Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
        if(rb == null)
            Debug.LogError("No Rigidbody2D found on " + gameObject.name);

        // Make sure gravity affects it
        rb.gravityScale = 1f;
        rb.freezeRotation = true; // prevents it from tipping over
    }

    void Update()
    {
        float moveInput = 0f;

        // Check keyboard input
        if(Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            moveInput = -speed;
        if(Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            moveInput = speed;

        // Move the box horizontally
        rb.velocity = new Vector2(moveInput, rb.velocity.y);
    }

    // Optional: just to see collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(gameObject.name + " hit " + collision.gameObject.name);
    }
}