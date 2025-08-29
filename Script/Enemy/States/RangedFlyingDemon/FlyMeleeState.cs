using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyMeleeState : State
{
    // next state
    public State meleeAttackState1;
    public State meleeAttackState2;
    public State flyChaseState;

    [Header("--- Fly Settings ---")]
    [SerializeField] private float flySpeed;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("--- References ---")]
    [SerializeField] private Transform selfTransform;
  

    private Animator animator;
    private Transform playerTransform;

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    public override State RunCurrState()
    {
        animator.SetFloat("MovementSpeed", 4f);
        // Rotate to look at the player
        Vector3 directionToPlayer = (playerTransform.position - selfTransform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        selfTransform.rotation = Quaternion.Slerp(selfTransform.rotation, lookRotation, Time.deltaTime * flySpeed);

        // Move forward in the direction it's facing
        selfTransform.Translate(Vector3.forward * flySpeed * Time.deltaTime);

        // Check for obstacles between enemy and player
        if (Physics.Linecast(selfTransform.position, playerTransform.position, obstacleLayer))
        {
            // If there's an obstacle, switch to flyChaseState
            return flyChaseState;
        }

        // Check the distance to the player
        float distanceToPlayer = Vector3.Distance(selfTransform.position, playerTransform.position);
        if (distanceToPlayer < attackRange)
        {
            // Choose randomly between the two attack states
            if (Random.value < 0.5f)
            {
                return meleeAttackState1;
            }
            else
            {
                return meleeAttackState2;
            }
        }

        // Remain in the current state
        return this;
    }
}
