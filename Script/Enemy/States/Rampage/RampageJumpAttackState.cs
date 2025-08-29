using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RampageJumpAttackState : State
{
    public State[] tauntStates;
    public State[] followingAttackStates;

    public float followAttackRange;

    public GameObject jumpSpell;
     
    public float beforeJump;
    public float afterJump;

    public Transform jumpSpawn;

    private bool isStarted = false;
    private bool isFinished = false;

    private Animator animator;
    private EnemyAudio enemyAudio;
    private Transform playerTransform;

    public Transform selfTransform;

    private NavMeshAgent agent;

    public GameObject[] fireHands;

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        enemyAudio = GetComponentInParent<EnemyAudio>();
        agent = GetComponentInParent<NavMeshAgent>();

        playerTransform = GameObject.FindWithTag("Player").transform;
        foreach(GameObject go in fireHands) go.SetActive(false);
    
    }

    public override State RunCurrState()
    {
        if (!isStarted) StartCoroutine(Jump());

        if (isFinished)
        {
            isFinished = false;
            isStarted = false;

            if (IsPlayerWithinRange(followAttackRange))
            {
                return followingAttackStates[Random.Range(0, followingAttackStates.Length)];
            }
            else
            {
                return tauntStates[Random.Range(0, tauntStates.Length)];
            }
        }

        return this;
    }

    private IEnumerator Jump()
    {
        isStarted = true;
        animator.SetTrigger("JumpAttack");
        enemyAudio.EffectSound();

        foreach (GameObject go in fireHands) go.SetActive(true);

        float elapsedTime = 0f;
        Quaternion startingRotation = selfTransform.rotation;
        Vector3 directionToPlayer = (playerTransform.position - selfTransform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        Vector3 startingPosition = selfTransform.position;
        Vector3 targetPosition = playerTransform.position;

        // Smoothly rotate and move towards the player during the beforeJump period
        while (elapsedTime < beforeJump)
        {
            // Rotate smoothly towards the player
            selfTransform.rotation = Quaternion.Slerp(startingRotation, targetRotation, elapsedTime / beforeJump);

            // Move towards the player
            selfTransform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / beforeJump);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final rotation and position are set
        selfTransform.rotation = targetRotation;
        selfTransform.position = targetPosition;

        // Calculate the direction from the Jump spawn point to the player
        Vector3 direction = (playerTransform.position - jumpSpawn.position).normalized;
        direction.y = 0; // Ensure the direction is only on the x and z axis

        // Calculate the rotation needed to face the player
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // Instantiate the Jump spell and set its rotation
        GameObject JumpInstance = Instantiate(jumpSpell, jumpSpawn.position, lookRotation, null);

        foreach (GameObject go in fireHands) go.SetActive(false);

        agent.isStopped = true;

         elapsedTime = 0f;
         startingRotation = selfTransform.rotation;
         directionToPlayer = (playerTransform.position - selfTransform.position).normalized;
         targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));

        while (elapsedTime < afterJump)
        {
            selfTransform.rotation = Quaternion.Slerp(startingRotation, targetRotation, elapsedTime / afterJump);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        selfTransform.rotation = targetRotation;

        isFinished = true;
    }
    private bool IsPlayerWithinRange(float range)
    {
        float distanceToPlayer = Vector3.Distance(selfTransform.position, playerTransform.position);
        return distanceToPlayer <= range;
    }
}
