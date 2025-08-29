using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("--- Settings ---")]
    [SerializeField] private float speed;
    [SerializeField] private GameObject explosion;

    private Rigidbody rb;
    private Vector3 direction;
    private bool isReleased = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    public void PrepareProjectile(Vector3 dir)
    { 
        direction = dir;
        // Rotate the projectile to face the given direction
        //transform.rotation = Quaternion.LookRotation(dir);
    }

    public void ReleaseProjectile()
    {
        isReleased = true;
    }

    private void Update()
    {
        if (isReleased)
        {
            // Move the projectile in the set direction
            rb.velocity = direction * speed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Instantiate the explosion at the collision point
        Instantiate(explosion, transform.position - transform.forward, Quaternion.identity, null);
        // Destroy the projectile
        Destroy(gameObject);
    }
}
