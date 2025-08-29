using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    private Animator animator;
    private List<Rigidbody> ragdollBodies = new List<Rigidbody>();
    private List<Collider> ragdollColliders = new List<Collider>();

    void Start()
    {
        animator = GetComponent<Animator>();
        MapRagdoll();
        SetRagdollState(false);
    }

    private void MapRagdoll()
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            if (rb.gameObject != this.gameObject && !rb.CompareTag("RagdollSkip"))
            {
                ragdollBodies.Add(rb);
                ragdollColliders.Add(rb.GetComponent<Collider>());
            }
        }
    }

    private void SetRagdollState(bool state)
    {
        foreach (Rigidbody rb in ragdollBodies)
        {
            rb.isKinematic = !state;
        }

        foreach (Collider col in ragdollColliders)
        {
            col.enabled = true;
            col.tag = state ? "DeadEnemy" : "Enemy";
        }

        if (animator != null)
        {
            animator.enabled = !state;
        }
    }

    public void EnableRagdoll()
    {
        SetRagdollState(true);
    }

    public void DisableRagdoll()
    {
        SetRagdollState(false);
    }
}
