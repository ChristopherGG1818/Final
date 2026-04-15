using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour
{
    public Collider2D hitbox;
    public float activeTime = 0.15f;
    public float offsetDistance = 1.5f;

    public Animator playerAnimator;

    private bool isAttacking;
    private Transform player;

    void Start()
    {
        hitbox.enabled = false;
        player = transform.parent;

        player = transform.parent;

    if (playerAnimator != null)
    {
        playerAnimator.speed = 10f;
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

        hitbox.enabled = true;

        yield return new WaitForSeconds(activeTime);

        hitbox.enabled = false;
        isAttacking = false;
    }
}