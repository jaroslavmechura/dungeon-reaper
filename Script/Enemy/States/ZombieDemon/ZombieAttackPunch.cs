using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttackPunch : State
{
    // --- Next State ---
    public State chaseState;
    public State kickState;

    [Header("--- Attack Settings ---")]
    [SerializeField] private float damage;
    [SerializeField] private float beforeHitTime;
    [SerializeField] private float afterHitTime;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private PlayerDamage damageImpactType;

    [Header("--- References ---")]
    [SerializeField] private Transform selfTransform;

    [Header("--- Continuous Kick ---")]
    [SerializeField] private float kickRange;

    private Animator animator;
    private bool isAttackingStart = false;
    private bool isAttackingEnd = false;
    private RaycastHit hit;
    private Transform playerTransform;
    private EnemyAudio enemyAudio;

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        playerTransform = GameObject.FindWithTag("Player").transform;
        enemyAudio = GetComponentInParent<EnemyAudio>();
    }

    public override State RunCurrState()
    {
        if (isAttackingEnd)
        {
            isAttackingStart = false;
            isAttackingEnd = false;

            if (Vector3.Distance(transform.position, playerTransform.position) < kickRange) {
                return kickState;
            }

            return chaseState;
        }

        if (!isAttackingStart) StartCoroutine(Attack());

        return this;
    }

    private IEnumerator Attack()
    {
        isAttackingStart = true;
        animator.SetTrigger("MeleeAttack1");
        enemyAudio.MeleeAttackSound();

        yield return new WaitForSeconds(beforeHitTime);

        Debug.DrawRay(transform.position + Vector3.up, transform.forward * attackRange, Color.yellow, 0.5f);
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, attackRange, playerLayer))
        {
            hit.collider.GetComponent<PlayerHealth>().TakeDamage(damage, damageImpactType);
        }

        float elapsedTime = 0f;
        Quaternion startingRotation = selfTransform.rotation;

        Vector3 directionToPlayer = playerTransform.position - selfTransform.position;
        directionToPlayer.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        while (elapsedTime < afterHitTime)
        {
            selfTransform.rotation = Quaternion.Slerp(startingRotation, targetRotation, elapsedTime / afterHitTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        selfTransform.rotation = targetRotation;

        isAttackingEnd = true;
    }

}
