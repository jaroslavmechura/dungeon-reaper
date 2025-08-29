using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasterTauntState : State
{
    public State idleState;

    public string triggerName;
    public float tauntLength;

    private bool isStarted = false;
    private bool isFinished = false;  

    private Animator animator;
    private EnemyAudio enemyAudio;

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        enemyAudio = GetComponentInParent<EnemyAudio>();
    }

    public override State RunCurrState()
    {
        if (!isStarted) 
        {
            StartCoroutine(Taunt());
        }

        if (isFinished) {
            isStarted = false;
            isFinished = false;

            return idleState;
        }


        return this;
    }

    private IEnumerator Taunt()
    {
        isStarted = true;

        animator.SetTrigger(triggerName);
        enemyAudio.ScreamSound();

        yield return new WaitForSeconds(tauntLength);

        isFinished = true;
    }
}
