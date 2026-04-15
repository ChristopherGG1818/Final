// using UnityEngine;

// public class Sword : MonoBehaviour
// {
//     public int damage = 1;

//     private bool hasHit;

//     private void OnEnable()
//     {
//         hasHit = false;
//     }

//     private void OnTriggerEnter2D(Collider2D collision)
//     {
//         if (hasHit) return;

//         if (collision.CompareTag("Enemy"))
//         {
//             hasHit = true;

//             collision.GetComponent<bugOneBehavior>()?.TakeDamage(damage);
//         }
//     }
// }