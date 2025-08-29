using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RampageChaseState : State
{
    [Header("--- States ---")]
    public State meleeState1;
    public State meleeState2;
    public State meleeState3;
    public State stompState;
    public State jumpAttackState;

    [Header("--- AttackTriggers ---")]
    [SerializeField] private float meleeAttackRange;
    [SerializeField] private float jumpAttackRange;

    [SerializeField] private LayerMask obstacleLayer;

    [Header("--- References ---")]
    [SerializeField] private Transform selfTransform;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform playerTransform;

    private bool canMelee = false;
    private bool canJump = false;

    public float turnSpeed;
    public float maxSpeed;
    private float currSpeed;

    private float distanceToPlayer;

    private void Start()
    {
        agent = GetComponentInParent<NavMeshAgent>();
        animator = GetComponentInParent<Animator>();
        playerTransform = GameObject.FindWithTag("Player").transform;

     
    }

    public override State RunCurrState()
    {
        if (canMelee) {
            agent.isStopped = true;
            canMelee = false;
            float random = Random.value;

            if (random < (1 / 4))
            {
                return meleeState1;
            }
            else if (random < (2 / 4))
            {
                return meleeState2;
            }
            else if (random < (3 / 4))
            {
                return meleeState3;
            }
            else 
            {
                return stompState;
            }
        }

        if (canJump) {
            
            canJump = false;
            return jumpAttackState;
        }

        if (agent.isStopped) agent.isStopped = false;
        agent.SetDestination(playerTransform.position);
        
        currSpeed = agent.velocity.magnitude;
        print("Magnitude: " + currSpeed.ToString());

        // Control braking and turning
        if (ShouldTurnRapidly())
        {
            PerformRapidTurn();
        }

        if (currSpeed < (maxSpeed/3)) 
        {
            animator.SetFloat("MovementSpeed", 0);
        }
        else if (currSpeed < ((maxSpeed/3) * 2)) 
        {
            animator.SetFloat("MovementSpeed", 2);
        }
        else 
        {
            animator.SetFloat("MovementSpeed", 4);
        }



        distanceToPlayer = Vector3.Distance(selfTransform.position, playerTransform.position);
        if (distanceToPlayer <= meleeAttackRange && IsPlayerInSight())
        {
            canMelee = true;
        }
        else if (distanceToPlayer <= jumpAttackRange && IsPlayerInSight())
        {
            canJump = true;
        }


        return this;
    }

    private bool IsPlayerInSight()
    {
        Vector3 directionToPlayer = (playerTransform.position - selfTransform.position).normalized;

        // Perform the linecast
        RaycastHit hit;
        if (Physics.Linecast(selfTransform.position, playerTransform.position, out hit, obstacleLayer))
        {
            // If the linecast hits an object that is not the player, return false
            if (hit.transform != playerTransform)
            {
                return false;
            }
        }

        // If no obstacles are hit or the hit is the player, return true
        return true;
    }

    private bool ShouldTurnRapidly()
    {
        // Define your condition for rapid turning
        // For example, if the angle between the agent's forward direction and the direction to the player is greater than a threshold
        Vector3 directionToPlayer = (playerTransform.position - selfTransform.position).normalized;
        float angle = Vector3.Angle(selfTransform.forward, directionToPlayer);
        return angle > 45f; // Adjust the angle threshold as needed
    }

    private void PerformRapidTurn()
    {
        // Reduce the agent's speed to simulate braking
        agent.velocity = agent.velocity * 0.95f; // Adjust the factor as needed
    }
}
