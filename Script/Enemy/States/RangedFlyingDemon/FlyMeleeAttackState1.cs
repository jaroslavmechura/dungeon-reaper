using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class FlyMeleeAttackState1 : State
{
    // --- Next State ---
    public State chaseState;
    public State attackState;

    [Header("--- Attack Settings ---")]
    [SerializeField] private float damage;
    [SerializeField] private float force;

    [SerializeField] private float beforeCollOn;
    [SerializeField] private float collOnLength;
    [SerializeField] private float afterCollOn;


    [SerializeField] private PlayerDamage damageImpactType;

    [Header("--- References ---")]
    [SerializeField] private Transform selfTransform;
    [SerializeField] private Collider weaponCollider;

    [Header("--- Continuous Attack ---")]
    [SerializeField] private float nextAttackRange;

    private Animator animator;
    private bool isAttackingStart = false;
    private bool isAttackingEnd = false;

    private Transform playerTransform;
    private EnemyAudio enemyAudio;

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        playerTransform = GameObject.FindWithTag("Player").transform;
        enemyAudio = GetComponentInParent<EnemyAudio>();
        weaponCollider.enabled = false;
    }

    public override void OnDeath()
    {
        weaponCollider.enabled = false;
    }

    public override State RunCurrState()
    {
        if (!isAttackingStart)
        {
            // Start attacking coroutine
            StartCoroutine(AttackCoroutine());
            isAttackingStart = true;
        }

        // Check if attacking has ended
        if (isAttackingEnd)
        {
            isAttackingStart = false;
            isAttackingEnd = false;

            // Check if player is within nextAttackRange
            float distanceToPlayer = Vector3.Distance(selfTransform.position, playerTransform.position);
            if (distanceToPlayer <= nextAttackRange )
            {
                return attackState;
            }
            else
            {
                return chaseState;
            }
        }

        return this;
    }

    private IEnumerator AttackCoroutine()
    {
        // Trigger attack animation
        animator.SetTrigger("MeleeAttack1");

        // Wait for beforeCollOn seconds
        yield return new WaitForSeconds(beforeCollOn);

        enemyAudio.MeleeAttackSound();

        // Enable collider
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }

        // Wait for collOnLength seconds
        yield return new WaitForSeconds(collOnLength);

        // Disable collider
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }

        // Wait for afterCollOn seconds
        yield return new WaitForSeconds(afterCollOn);

        // Finish attack
        isAttackingEnd = true;
    }

    public override void DealDamage() {
        playerTransform.GetComponent<PlayerHealth>().TakeDamage(damage, damageImpactType);

        Vector3 forceDirection = (playerTransform.position - weaponCollider.transform.position).normalized;
        forceDirection.y = 0.05f;
        playerTransform.GetComponent<Rigidbody>().AddForce(forceDirection * force, ForceMode.Impulse);
    }
}
