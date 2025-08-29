using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasterStompState : State
{
    public State teleportState;

    public GameObject stompSpell;

    public float beforeStomp;
    public float afterStomp;

    public Transform stompSpawn;

    private bool isStarted = false;
    private bool isFinished = false;

    private Animator animator;
    private EnemyAudio enemyAudio;
    private Transform playerTransform;
  

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        enemyAudio = GetComponentInParent<EnemyAudio>();

        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    public override State RunCurrState()
    {
        if (!isStarted) StartCoroutine(Stomp());

        if (isFinished) {
            isFinished = false;
            isStarted = false;
            return teleportState;
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

        yield return new WaitForSeconds(afterStomp);

        isFinished = true;
    }
}
