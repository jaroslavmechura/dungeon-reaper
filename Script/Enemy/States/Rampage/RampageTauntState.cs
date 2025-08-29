using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampageTauntState : State
{
    public State chaseState;

    [Header("--- Taunt Settings ---")]
    [SerializeField] private string animationTrigger;
    [SerializeField] private float tauntDuration;


    private EnemyAudio enemyAudio;
    private Animator animator;

    private bool isStarted = false;
    private bool isEnded = false;

    private void Start()
    {
        enemyAudio = GetComponentInParent<EnemyAudio>();
        animator = GetComponentInParent<Animator>();
    }

    public override State RunCurrState()
    {

        if (!isStarted) StartCoroutine(Taunt());

        if (isEnded) 
        {
            isStarted = false;
            isEnded = false;
            return chaseState;
        }

        return this;
    }

    private IEnumerator Taunt() {
        isStarted = true;
        animator.SetTrigger(animationTrigger);
        enemyAudio.ScreamSound();

        yield return new WaitForSeconds(tauntDuration);

        isEnded = true; 
    }
}
