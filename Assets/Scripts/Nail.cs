using UnityEngine;

public class Sword : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Hit enemy!");

            // Call enemy damage function (only if it exists)
            collision.GetComponent<EnemyHealth>()?.TakeDamage(damage);
        }
    }
}