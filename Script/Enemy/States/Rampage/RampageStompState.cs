using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampageStompState : State
{
    public State[] tauntStates;
    public State[] followingAttackStates;

    public float followAttackRange;

    public GameObject stompSpell;

    public float beforeStomp;
    public float afterStomp;

    public Transform stompSpawn;

    private bool isStarted = false;
    private bool isFinished = false;

    private Animator animator;
    private EnemyAudio enemyAudio;
    private Transform playerTransform;

    public Transform selfTransform;
    


    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        enemyAudio = GetComponentInParent<EnemyAudio>();

        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    public override State RunCurrState()
    {
        if (!isStarted) StartCoroutine(Stomp());

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

    private IEnumerator Stomp()
    {
        isStarted = true;
        animator.SetTrigger("Stomp");
        enemyAudio.EffectSound();

        yield return new WaitForSeconds(beforeStomp);

        // Calculate the direction from the stomp spawn point to the player
        Vector3 direction = (playerTransform.position - stompSpawn.position).normalized;
        direction.y = 0; // Ensure the direction is only on the x and z axis

        // Calculate the rotation needed to face the player
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // Instantiate the stomp spell and set its rotation
        GameObject stompInstance = Instantiate(stompSpell, stompSpawn.position, lookRotation, null);

        float elapsedTime = 0f;
        Quaternion startingRotation = selfTransform.rotation;
        Vector3 directionToPlayer = (playerTransform.position - selfTransform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));

        while (elapsedTime < afterStomp)
        {
            selfTransform.rotation = Quaternion.Slerp(startingRotation, targetRotation, elapsedTime / afterStomp);
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
