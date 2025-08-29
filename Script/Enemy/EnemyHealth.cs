using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("--- Health Settings ---")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float currHealth;
    [SerializeField] private float goreSplashLimit;

    [Header("--- References ---")]
    [SerializeField] private Rigidbody torsoRb;

    private RagdollManager ragdollManager;
    private StateManager stateManager;
    private Animator animator;
    private NavMeshAgent agent;
    private EnemyAudio enemyAudio;

    public bool isDead;

    public GameObject gore;
    public bool isRagdol;
    public bool isHealthRigged = false;


    private void Start()
    {
        if (!isHealthRigged) {currHealth = maxHealth;}else { currHealth = 3f; }
        ragdollManager = GetComponent<RagdollManager>();
        stateManager = GetComponentInChildren<StateManager>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        enemyAudio = GetComponent<EnemyAudio>();

        isDead = false;
    }

    private void Update()
    {
        if (currHealth <= 0f && !isDead) Death();       
    }

    public void Death() {  
        if (isRagdol && currHealth > goreSplashLimit)
        {
            stateManager.enabled = false;
            ragdollManager.EnableRagdoll();
            if (agent != null) agent.isStopped = true;
            torsoRb.AddForce(((-torsoRb.transform.forward) + (torsoRb.transform.up / 2)) * 100f, ForceMode.Impulse);
            isDead = true;
            enemyAudio.enabled = false;
        }
        else {
            stateManager.enabled = false;
            if (agent != null) agent.isStopped = true;
            isDead = true;
            enemyAudio.enabled = false;
            Instantiate(gore, transform.position + Vector3.up, Quaternion.identity, null);

            StateManager stateManaager = GetComponentInChildren<StateManager>();
            stateManaager.currState.OnDeath();
            

            Destroy(gameObject);
        }

    }

    public void TakeDamage(float dmg, bool isShotgun, bool isAudio) {
        currHealth -= dmg;
        animator.SetTrigger("TakeDamage");
        if (isShotgun)
        {
            if (Random.Range(0, 100) > 49) enemyAudio.HurtSound();
        }
        else if (isAudio) {
                enemyAudio.HurtSound();
        }
      
    }

    public void RiggHealth() 
    {
        isHealthRigged = true;
        currHealth = 1f;
        goreSplashLimit = -1f;
    }
}
