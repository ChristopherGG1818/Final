using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour
{
    public Collider2D hitbox;
    public float activeTime = 0.2f;

    [Header("Follow Player")]
    public Transform player;

    private bool isAttacking;
    public Vector3 offset = new Vector3(0.58f, 0f, 0f);
    

    void Start()
    {
        hitbox.enabled = false;
    }

    void Update()
    {
        // Keep sword locked to player position
        if (player != null)
        {
            transform.position = player.position + offset;
        }
    }

    public void Attack()
    {
        if (isAttacking) return;

        StartCoroutine(Slash());
    }

    IEnumerator Slash()
    {
        isAttacking = true;

        hitbox.enabled = true;

        float timer = 0f;
        while (timer < activeTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        hitbox.enabled = false;

        isAttacking = false;
    }
}