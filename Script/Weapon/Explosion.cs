using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [Header("--- Explosion Settings ---")]
    [SerializeField] private float range;
    [SerializeField] private float damage;
    [SerializeField] private float forceStrength;
    [SerializeField] private LayerMask explosionLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float destroyDelay;
    [SerializeField] private PlayerDamage damageImpactType;
    [Header("--- Impact On Player ---")]
    [SerializeField] private float damageFactor;
    [SerializeField] private float forceFactor;

    private void Start()
    {
        Explode();
    }

    private void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range, explosionLayer);
        HashSet<GameObject> affectedEnemies = new HashSet<GameObject>();

        foreach (var hitCollider in hitColliders)
        {
            GameObject hitObject = hitCollider.transform.root.gameObject;
            if (affectedEnemies.Contains(hitObject))
            {
                continue;
            }

            RaycastHit hit;
            Vector3 direction = hitCollider.transform.position - transform.position;
            Physics.Raycast(transform.position, direction, out hit, range, explosionLayer);

            // Calculate distance from explosion center to the object
            float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
            float distanceFactor = Mathf.Clamp01(1 - (distance / range)); // Factor decreases with distance

            if (hitCollider.CompareTag("DeadEnemy"))
            {
                Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(direction.normalized * forceStrength * distanceFactor, ForceMode.Impulse);
                }

                // visual impact
                Instantiate(hitCollider.GetComponentInParent<EnemyController>().impactParticle, hit.point, Quaternion.LookRotation(hit.normal), null);
                affectedEnemies.Add(hitObject);
            }
            else if (hitCollider.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hitCollider.GetComponentInParent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage * distanceFactor, false, true); // Apply damage based on distance
                }

                // visual impact
                Instantiate(hitCollider.GetComponentInParent<EnemyController>().impactParticle, hit.point, Quaternion.LookRotation(hit.normal), null);
                affectedEnemies.Add(hitObject);
            }
            else if (hitCollider.CompareTag("DynamicProp"))
            {
                Rigidbody dynamicPropRb = hitCollider.GetComponent<Rigidbody>();
                if (dynamicPropRb == null)
                {
                    dynamicPropRb = hitCollider.GetComponentInParent<Rigidbody>();
                }

                if (dynamicPropRb != null)
                {
                    dynamicPropRb.AddForce(direction.normalized * forceStrength * distanceFactor, ForceMode.Impulse);
                }
            }/*
            else if (hitCollider.CompareTag("Interactive"))
            {
                hitCollider.GetComponent<InteractiveObject>().TakeDamage();
            }*/
            else if (hitCollider.CompareTag("Player"))
            {
                hitCollider.GetComponent<Rigidbody>().AddForce((direction.normalized * forceStrength * distanceFactor) * forceFactor, ForceMode.Impulse);
                hitCollider.GetComponent<PlayerHealth>().TakeDamage((damage * distanceFactor) * damageFactor, damageImpactType);
            }
        }

        // Destroy explosion object after applying effects
        Destroy(gameObject, destroyDelay);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
