using UnityEngine;

public class bugProjectile : MonoBehaviour
{
    public float speed = 6f;
    public float lifeTime = 4f;

    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifeTime); // auto destroy after lifetime
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Apply knockback to the player
            playerBehavior player = collision.GetComponent<playerBehavior>();
            if (player != null)
            {
                player.ApplyKnockback(transform.position); // knockback away from projectile
            }

            Destroy(gameObject); // Destroy projectile after hitting player
        }

        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject); // Destroy projectile on walls
        }
    }
}