using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyChaseState : State
{ 
    // --- Next State ---
    public State rangedAttackState;
    public State meleeAttackState;

    [Header("--- Movement Settings ---")]
    [SerializeField] private float speed;
    [SerializeField] private float minDistance;

    [Header("--- Ranged Attack Check Settings ---")]
    [SerializeField] private float rangedAttackRange;
    [SerializeField] private float rangedAttackCooldown;

    [Header("--- Melee Attack Settings ---")]
    [SerializeField] private float meleeAttackRange;
    [SerializeField] private float meleeAttackCooldown;

    [Header("--- References ---")]
    [SerializeField] private Transform selfTransform;

    [Header("--- Obstacle Avoidance Settings ---")]
    [SerializeField] private float obstacleAvoidanceRange = 5f;
    [SerializeField] private float obstacleAvoidanceForce = 1f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float[] obstacleOffsets;


    private float rangedAttackCooldownTimer;
    private float meleeAttackCooldownTimer;
    private bool canAttackRanged;
    private bool canAttackMelee;


    private Animator animator;

    // --- Navigation ---
    private Transform playerTransform;
    private float originalSpeed;
    private Vector3 direction;
    private Vector3 refDir;
    private RaycastHit hit;
    private Vector3[] offsets;
    private int[] closeObstacles;
    private int[] closedObstaclesEmpty = new int[4] { 0, 0, 0, 0 };
    private int dirId;

    private int minValue;
    private int minIndex;
    private int isMax;
    private int looseEnd;

    private void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        animator = GetComponentInParent<Animator>();
        originalSpeed = speed;
    }

    public override State RunCurrState()
    {
        if (canAttackMelee)
        {
            rangedAttackCooldownTimer = rangedAttackCooldown;
            meleeAttackCooldownTimer = meleeAttackCooldown;
            canAttackRanged = false;
            canAttackMelee = false;
            return meleeAttackState;
        }

        if (canAttackRanged)
        {
            rangedAttackCooldownTimer = rangedAttackCooldown;
            canAttackRanged = false;
            canAttackMelee = false;
            return rangedAttackState;
        }

        Move();
        CheckAttack();
        HandleCooldown();

        return this;
    }

    private void Move()
    {
        CheckForObstacles();

        selfTransform.position += (direction * speed * Time.deltaTime);

        Quaternion lookRotation = Quaternion.LookRotation(playerTransform.position - selfTransform.position);
        selfTransform.rotation = Quaternion.Slerp(selfTransform.rotation, lookRotation, Time.deltaTime * speed);
    }

    private void CheckForObstacles()
    {
        refDir = ((playerTransform.position + playerTransform.up) - selfTransform.position).normalized;

        // Central raycast
        if (Physics.Raycast(selfTransform.position, selfTransform.forward, out hit, obstacleAvoidanceRange, obstacleLayer))
        {
            Debug.DrawRay(selfTransform.position, selfTransform.forward * obstacleAvoidanceRange, Color.red);
            direction = Vector3.zero;
        }
        else
        { // no obstacle - just fly towards player
            Debug.DrawRay(selfTransform.position, selfTransform.forward * obstacleAvoidanceRange, Color.green);
            direction = refDir;
            return;
        }

        closeObstacles = closedObstaclesEmpty;

        foreach (float offset in obstacleOffsets)
        {
            // Offsets for right, left, up, and down 
            offsets = new Vector3[]
            {
            selfTransform.right * offset,
            -selfTransform.right * offset,
            selfTransform.up * offset,
            -selfTransform.up * offset
            };

            dirId = -1;

            foreach (var offsetDir in offsets)
            {
                dirId++;

                // Check from enemy to the offset start point
                if (Physics.Raycast(selfTransform.position, offsetDir, out hit, offset, obstacleLayer))
                {
                    Debug.DrawRay(selfTransform.position, offsetDir, Color.red);
                    closeObstacles[dirId]++;
                    continue;
                }
                else
                {
                    // Perform the actual offset raycast if the path to the offset start point is clear
                    if (Physics.Raycast(selfTransform.position + offsetDir, selfTransform.forward, out hit, obstacleAvoidanceRange, obstacleLayer))
                    {
                        Debug.DrawRay(selfTransform.position + offsetDir, selfTransform.forward * obstacleAvoidanceRange, Color.red);
                        continue;
                    }
                    else
                    {
                        Debug.DrawRay(selfTransform.position + offsetDir, selfTransform.forward * obstacleAvoidanceRange, Color.green);
                        direction = offsetDir.normalized * obstacleAvoidanceForce;
                        return;
                    }
                }
            }
        }

        looseEnd = FindIndexOfMinValue();

        if (looseEnd == -1)
        {
            direction = -refDir;
            return;
        }

        direction = offsets[looseEnd].normalized * obstacleAvoidanceForce;
    }

    private void CheckAttack()
    {
        if (Vector3.Distance(selfTransform.position, playerTransform.position) <= meleeAttackRange && meleeAttackCooldownTimer <= 0f && !Physics.Linecast(selfTransform.position, playerTransform.position, obstacleLayer))
        { 
            canAttackMelee = true;
            animator.SetFloat("MovementSpeed", 4f);
        }
        else if (Vector3.Distance(selfTransform.position, playerTransform.position) <= rangedAttackRange && rangedAttackCooldownTimer <= 0f && !Physics.Linecast(selfTransform.position, playerTransform.position, obstacleLayer))
        {
            canAttackRanged = true;
            animator.SetFloat("MovementSpeed", 0f);
        }
        else if (Vector3.Distance(selfTransform.position, playerTransform.position) <= minDistance) 
        {
            animator.SetFloat("MovementSpeed", 0f);
            speed = 0f;
        }
        else
        {
            speed = originalSpeed;
            animator.SetFloat("MovementSpeed", 2f);
        }
    }

    private void HandleCooldown()
    {
        if (rangedAttackCooldownTimer >= 0f)
        {
            rangedAttackCooldownTimer -= Time.deltaTime;
        }

        if (meleeAttackCooldownTimer >= 0f) 
        {
            meleeAttackCooldownTimer -= Time.deltaTime;
        }
    }

    int FindIndexOfMinValue()
    {
        if (closeObstacles == null || closeObstacles.Length == 0)
        {
            throw new System.ArgumentException("Array cannot be null or empty");
        }

        minValue = closeObstacles[0];
        minIndex = 0;

        for (int i = 1; i < closeObstacles.Length; i++)
        {
            if (closeObstacles[i] < minValue)
            {
                minValue = closeObstacles[i];
                minIndex = i;
            }
            if (closeObstacles[i] == obstacleOffsets.Length) isMax++;
        }

        if (isMax == offsets.Length) return -1;

        return minIndex;
    }



}
