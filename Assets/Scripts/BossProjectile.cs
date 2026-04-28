using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerBehavior player = collision.GetComponent<playerBehavior>();

            if (player != null)
            {
                player.ApplyKnockback(transform.position);
            }

            Destroy(gameObject);
        }
    }
}