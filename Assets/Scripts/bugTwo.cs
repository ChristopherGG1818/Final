using UnityEngine;

public class bugTwo : MonoBehaviour, IDamageable
{
    [Header("Dummy Mode")]
    public bool isDummy = true;

    [Header("Health")]
    public int health = 3;

    private SpriteRenderer sr;
    private Color originalColor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (sr != null)
            originalColor = sr.color;
    }

    void Update()
    {
        // Literally does nothing
        if (isDummy) return;
    }

    // DAMAGE ONLY (no movement, no knockback, no behavior)
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (sr != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashRed());
        }

        if (health <= 0)
            Destroy(gameObject);
    }

    System.Collections.IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = originalColor;
    }

    // Optional: ignore player collision entirely
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDummy) return;

        // (nothing happens anyway)
    }
}