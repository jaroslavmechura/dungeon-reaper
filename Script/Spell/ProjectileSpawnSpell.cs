using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawnSpell : Spell
{
    [Header("--- Spell Settings ---")]
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private GameObject basicImpact;
    [SerializeField] private float activateColliderTimer;
    [SerializeField] private PlayerDamage damageImpactType;
    [Header("--- Sub Spells ---")]
    [SerializeField] private GameObject spellToSpawn;
    [SerializeField] private int spellsToSpawnCount;

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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        selfCollider = GetComponent<Collider>();
        playerTransform = GameObject.FindWithTag("Player").transform;
        startScale = 0f;
        transform.localScale = startScale * Vector3.one;
    }

    public override void CreateSpell(float duration)
    {
        creationAudioSource.Play();
        StartCoroutine(HandleCreation(duration));
    }

    public override void UseSpell()
    {
        rb.useGravity = true;

        // Get the direction vector and distance to the player
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // Set the height at which the projectile should reach its peak
        float height = 0.75f; // Adjust this value as needed

        // Calculate the initial velocity needed to reach the target with a parabolic trajectory
        float gravity = Physics.gravity.magnitude;
        float initialVelocity = Mathf.Sqrt((gravity * distance * distance) / (2 * (distance * Mathf.Tan(Mathf.Deg2Rad * 45) - height)));

        // Apply a force multiplier to extend the range
        float forceMultiplier = 1.25f; // Adjust this value as needed to fine-tune the distance
        initialVelocity *= forceMultiplier;

        // Split the velocity into horizontal and vertical components
        Vector3 horizontalVelocity = direction * initialVelocity * Mathf.Cos(Mathf.Deg2Rad * 45);
        Vector3 verticalVelocity = Vector3.up * initialVelocity * Mathf.Sin(Mathf.Deg2Rad * 45);

        // Set the initial velocity of the Rigidbody
        rb.velocity = horizontalVelocity + verticalVelocity;

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


    private IEnumerator ActivateCollider()
    {
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

    private IEnumerator SpawnProjectiles(float thisduration)
    {
        // Calculate the interval between each projectile spawn
        float fireRate = thisduration / spellsToSpawnCount;
        float partFireRate = fireRate / 4;

        for (int i = 0; i < spellsToSpawnCount; i++)
        {
            // Instantiate the spell
            GameObject spawnedSpellObj = Instantiate(spellToSpawn, transform.position, Quaternion.identity, null);
            Spell spell = spawnedSpellObj.GetComponent<Spell>();

            // Start spell creation
            spell.CreateSpell(partFireRate);

            // Wait for half of the fire rate duration
            yield return new WaitForSeconds(partFireRate);

            // Use the spell
            spell.UseSpell();

            // Wait for the other half of the fire rate duration before spawning the next spell
            yield return new WaitForSeconds(partFireRate * 3);
        }

    }

    private IEnumerator HandleCreation(float thisduration) 
    {
        StartCoroutine(ScaleSpell(thisduration / 3));

        yield return new WaitForSeconds(thisduration / 3);

        StartCoroutine(SpawnProjectiles((thisduration / 3) * 2));
    }
}
