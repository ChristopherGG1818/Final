using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour
{
    public Collider2D hitbox;
    public float activeTime = 0.15f;
    public float offsetDistance = 1.5f;

    public Animator playerAnimator;

    private bool isAttacking;
    private bool hasHit; // NEW
    private Transform player;

    void Start()
    {
        hitbox.enabled = false;
        player = transform.parent;

        if (playerAnimator != null)
        {
            playerAnimator.speed = 10f; // you can lower later if needed
        }
    }

    void Update()
    {
        if (player == null) return;

        float facing = Mathf.Sign(player.localScale.x);

        transform.localPosition = new Vector3(
            offsetDistance * facing,
            0.5f,
            0f
        );
    }

    public void Attack()
    {
        Debug.Log("Sword Attack Called!");
        if (isAttacking) return;

        playerAnimator.SetTrigger("Attack");
        StartCoroutine(Slash());
    }

    IEnumerator Slash()
    {
        isAttacking = true;
        hasHit = false; // RESET EACH SWING

        hitbox.enabled = true;

        yield return new WaitForSeconds(activeTime);

        hitbox.enabled = false;
        isAttacking = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return; // PREVENT MULTIPLE HITS

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Hit enemy!");

            hasHit = true; // mark as already hit

            other.GetComponent<bugOneBehavior>()?.TakeDamage(1);
        }
    }
}