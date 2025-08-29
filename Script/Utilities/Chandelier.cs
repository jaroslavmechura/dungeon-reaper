using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chandelier : MonoBehaviour
{
    public Collider fallDamageCollider;

    private void Start()
    {
        if (fallDamageCollider != null)
        {
            fallDamageCollider.enabled = false;
        }
    }

    public void StartFalling() 
    {
        fallDamageCollider.enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!fallDamageCollider.enabled) return;
        if (collision.gameObject == gameObject) return;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponentInParent<EnemyHealth>().TakeDamage(1000000f, false, true);
        }
        else 
        {
            fallDamageCollider.enabled = false;
        }
    }
}

