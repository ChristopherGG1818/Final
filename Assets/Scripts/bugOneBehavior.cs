using UnityEngine;

public class bugOneBehavior : MonoBehaviour
{
    public float speed = 2f;

    // Direction change timing
    public float minMoveTime = 1f;
    public float maxMoveTime = 3f;

    // Movement boundaries
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    private Vector2 moveDirection;
    private float moveTimer;

    void Start()
    {
        ChooseNewDirection();
    }

    void Update()
    {
        MoveBug();
        HandleTimer();
        ClampPosition();
    }

    void MoveBug()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);
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

    void ClampPosition()
    {
        Vector3 pos = transform.position;

        bool hitBoundary = false;

        if (pos.x < minX || pos.x > maxX)
            hitBoundary = true;

        if (pos.y < minY || pos.y > maxY)
            hitBoundary = true;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;

        // If it touched boundary → change direction
        if (hitBoundary)
            ChooseNewDirection();
    }

    // 🔥 When hitting another object
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ChooseNewDirection();
    }
}