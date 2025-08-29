using System.Collections;
using UnityEngine;

public class ZombieScream : State
{
    // next state 
    public State chaseState;

    [Header("--- Scream Settings ---")]
    [SerializeField] private float screamLengthBefore;
    [SerializeField] private float screamLengthAfter;


    private bool isScreamStart = false;
    private bool isScreamEnd = false;
    private Animator animator;
    private EnemyAudio enemyAudio;

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        enemyAudio = GetComponentInParent<EnemyAudio>();
    }

    public override State RunCurrState()
    {
        if (isScreamEnd)
        {
            isScreamEnd = false;
            isScreamStart = false;

            return chaseState;
        }

        else if (!isScreamStart) 
        { 
            isScreamStart = true;
            StartCoroutine(Scream());
        }

        return this;
    }
     
    private IEnumerator Scream() {
        animator.SetTrigger("Scream");

        yield return new WaitForSeconds(screamLengthBefore);

        enemyAudio.ScreamSound();

        yield return new WaitForSeconds(screamLengthAfter);

        isScreamEnd = true;
    }
}
