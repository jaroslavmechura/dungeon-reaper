using System.Collections;
using UnityEngine;

public class RampageMeleeState : State
{
    public State[] tauntStates;
    public State[] followAttackStates;

    [Header("--- Attack Settings ---")]
    [SerializeField] private float damage;
    [SerializeField] private float force;

    [SerializeField] private string animationTrigger;

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
        enemyAudio = GetComponentInParent<EnemyAudio>();
        playerTransform = GameObject.FindWithTag("Player").transform;

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
            if (distanceToPlayer <= nextAttackRange)
            {
                return followAttackStates[Random.Range(0, followAttackStates.Length)];
            }
            else
            {
                return tauntStates[Random.Range(0, tauntStates.Length)];
            }
        }

        return this;
    }

    public override void OnDeath()
    {
        weaponCollider.enabled = false;
    }

    private IEnumerator AttackCoroutine()
    {
  

        // Trigger attack animation
        animator.SetTrigger(animationTrigger);

        // Wait for beforeCollOn seconds
        yield return new WaitForSeconds(beforeCollOn);

        enemyAudio.MeleeAttackSound();

        // Enable collider
  
            weaponCollider.enabled = true;
        

        // Wait for collOnLength seconds
        yield return new WaitForSeconds(collOnLength);

        // Disable collider

            weaponCollider.enabled = false;
        

        float elapsedTime = 0f;
        Quaternion startingRotation = selfTransform.rotation;
        Vector3 directionToPlayer = (playerTransform.position - selfTransform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));

        while (elapsedTime < afterCollOn)
        {
            selfTransform.rotation = Quaternion.Slerp(startingRotation, targetRotation, elapsedTime / afterCollOn);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        selfTransform.rotation = targetRotation;

        // Finish attack
        isAttackingEnd = true;

        
    }

    public override void DealDamage()
    {
        playerTransform.GetComponent<PlayerHealth>().TakeDamage(damage, damageImpactType);

        Vector3 forceDirection = (playerTransform.position - weaponCollider.transform.position).normalized;
        forceDirection.y = 0.05f;
        playerTransform.GetComponent<Rigidbody>().AddForce(forceDirection * force, ForceMode.Impulse);
    }
}
