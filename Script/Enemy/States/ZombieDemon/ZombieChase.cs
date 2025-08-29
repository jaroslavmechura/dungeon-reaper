using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieChase : State
{
    // --- Next State ---
    public State attackStatePunch;
    public State attackStateKick;
    public State screamState;

    [Header("--- Chase Settings ---")]
    [SerializeField] private float attackRangePunch;
    [SerializeField] private float attackRangeKick;

    [Header("--- Scream Settings ---")]
    [SerializeField] private float minScreamTimer;
    [SerializeField] private float maxScreamTimer;

    private float screamTimer;

    private GameObject player;
    private NavMeshAgent agent;
    private Animator animator;
    private bool canAttackPunch;
    private bool canAttackKick;

    private float distance;


    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        agent = GetComponentInParent<NavMeshAgent>();
        animator = GetComponentInParent<Animator>();

        screamTimer = Random.Range(minScreamTimer, maxScreamTimer);
    }

    public override State RunCurrState()
    {
        if (canAttackKick)
        {
            canAttackKick = false;
            canAttackPunch = false;
            return attackStateKick;
        }

        else if (canAttackPunch)
        {
            canAttackKick = false;
            canAttackPunch = false;
            return attackStatePunch;
        }

        else if (screamTimer < 0f) {
            agent.isStopped = true;
            screamTimer = Random.Range(minScreamTimer, maxScreamTimer);
            canAttackKick = false;
            canAttackPunch = false;
            return screamState;
        }

        Move();
        CheckAttack();

        screamTimer -= Time.deltaTime;

        return this;
    }

    private void Move()
    {
        if (agent.isStopped) agent.isStopped = false;

        agent.SetDestination(player.transform.position);
    }

    private void CheckAttack()
    {
        distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < attackRangeKick)
        {
            agent.isStopped = true;
            canAttackKick = true;
        }
        else if (distance < attackRangePunch) 
        {
            agent.isStopped = true;
            canAttackPunch = true;
        }
    }
}
