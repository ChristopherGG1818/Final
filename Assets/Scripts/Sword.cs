using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour
{
    public Collider2D hitbox;
    public float activeTime = 0.15f;
    public float offsetDistance = 1.5f;

    public Animator playerAnimator;

    private bool isAttacking;
    private bool hasHit;
    private Transform player;

    private SpriteRenderer sr;

    void Start()
    {
        hitbox.enabled = false;
        player = transform.parent;

        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.enabled = false; // hide sword at start
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

        isAttacking = true;

        if (sr != null)
            sr.enabled = true; // show sword

        playerAnimator.SetTrigger("Attack");

        StartCoroutine(Slash());
    }

    IEnumerator Slash()
    {
        hasHit = false;

        hitbox.enabled = true;

        yield return new WaitForSeconds(activeTime);

        hitbox.enabled = false;

        isAttacking = false;

        if (sr != null)
            sr.enabled = false; // hide sword
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        if (other.CompareTag("Enemy"))
        {
            hasHit = true;

            Debug.Log("Hit enemy!");

            other.GetComponent<bugOneBehavior>()?.TakeDamage(1);
        }
    }
}