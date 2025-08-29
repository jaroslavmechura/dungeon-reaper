using System.Collections.Generic;
using UnityEngine;

public class ParticleCollisionHandler : MonoBehaviour
{
    public int everyIsh;
    public GameObject decalPrefab;
    private ParticleSystem part;
    private List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();

        if (everyIsh == 0) everyIsh = 1;
    }

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < numCollisionEvents; i+=everyIsh) 
        {
            Vector3 collisionPos = collisionEvents[i].intersection;
            Vector3 collisionNormal = collisionEvents[i].normal;

            // Check if the collision point is inside the collider
            Collider collider = other.GetComponent<Collider>();
            if (collider != null)
            {
                Vector3 closestPoint = collider.ClosestPoint(collisionPos);
                if (closestPoint != collisionPos)
                {
                    // The point is inside the collider, flip the normal
                    collisionNormal = -collisionNormal;
                }
            }

            Quaternion rotation = Quaternion.LookRotation(collisionNormal);

            GameObject decal = Instantiate(decalPrefab, collisionPos, rotation);

            float randomZRotation = Random.Range(0f, 360f);
            decal.transform.Rotate(0, 0, randomZRotation, Space.Self);
        }
    }
}