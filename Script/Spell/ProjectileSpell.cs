using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class ProjectileSpell : Spell
{
    [Header("--- Spell Settings ---")]
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private GameObject basicImpact;
    [SerializeField] private float activateColliderTimer;
    [SerializeField] private PlayerDamage damageImpactType;
    [Header("--- Spell Creation ---")]
    [SerializeField] private float startScale;
    [SerializeField] private float finalScale;

    [Header("--- Audio ---")]
    [SerializeField] private AudioSource creationAudioSource;

    private Rigidbody rb;

    private Collider hitCollider;
    private Vector3 hitNormal;
    private Vector3 hitPoint;
    private Vector3 spellDirection;

    private Collider selfCollider;
    private Transform playerTransform; 
    


    public override void CreateSpell(float duration)
    {
        rb = GetComponent<Rigidbody>();
        selfCollider = GetComponent<Collider>();
        playerTransform = GameObject.FindWithTag("Player").transform.Find("Head").transform;
        startScale = 0f;
        transform.localScale = startScale * Vector3.one;

        StartCoroutine(ScaleSpell(duration));
        creationAudioSource.Play();


        Vector3 directionToTorso = playerTransform.position - transform.position;

        // Rotate the projectile to face the player's torso
        transform.rotation = Quaternion.LookRotation(directionToTorso);
    }

    public override void UseSpell()
    {
        Vector3 directionToTorso = playerTransform.position - transform.position;

        // Calculate the normalized direction
        spellDirection = directionToTorso.normalized;

        // Set the velocity of the Rigidbody
        rb.velocity = spellDirection * speed;

        // Start the collider activation coroutine
        StartCoroutine(ActivateCollider());
    }

    private void OnCollisionEnter(Collision collision)
    {
        hitCollider = collision.collider;
        hitNormal = collision.contacts[0].normal;
        hitPoint = collision.contacts[0].point;

        if (hitCollider.CompareTag("DeadEnemy"))
        {
            hitCollider.GetComponent<Rigidbody>().AddForce(-hitNormal * speed, ForceMode.Impulse);

            // visual impact
            Instantiate(hitCollider.GetComponentInParent<EnemyController>().impactParticle, hitPoint, Quaternion.LookRotation(hitNormal), null);
        }
        else if (hitCollider.CompareTag("Enemy"))
        {
            hitCollider.GetComponentInParent<EnemyHealth>().TakeDamage(damage, false, true);

            // visual impact
            Instantiate(hitCollider.GetComponentInParent<EnemyController>().impactParticle, hitPoint, Quaternion.LookRotation(hitNormal), null);
        }
        else if (hitCollider.CompareTag("DynamicProp"))
        {
            hitCollider.GetComponent<Rigidbody>().AddForce(-hitNormal * speed, ForceMode.Impulse);
        }
        else if (hitCollider.CompareTag("Player")) 
        {
            hitCollider.GetComponent<PlayerHealth>().TakeDamage(damage, damageImpactType);
        }
        // visual impact
        Instantiate(basicImpact, hitPoint, Quaternion.LookRotation(hitNormal), null);
        Destroy(gameObject);
    }


    private IEnumerator ActivateCollider() {
        yield return new WaitForSeconds(activateColliderTimer);

        selfCollider.enabled = true;
    }

    private IEnumerator ScaleSpell(float duration)
    {
        float elapsedTime = 0f;
        Vector3 initialScale = startScale * Vector3.one;
        Vector3 targetScale = finalScale * Vector3.one;

        while (elapsedTime < duration)
        {
            float scaleProgress = elapsedTime / duration;
            transform.localScale = Vector3.Lerp(initialScale, targetScale, scaleProgress);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }
}
