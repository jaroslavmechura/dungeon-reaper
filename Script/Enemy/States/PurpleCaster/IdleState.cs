using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IdleState : State
{
    // --- Next State ---
    public State stompState;
    public State tauntState1;
    public State tauntState2;
    public State[] spellCastSingleState;
    public State teleportState;

    public float meleeAttackRange;
    public float attackCooldown;
    public float attackTimer;

    public float tauntCooldownMin;
    public float tauntCooldownMax;
    public float tauntTimer;

    private Transform playerTransform;
    private Animator animator;
    public Transform selfTransform;

    private int lastSpellIndex = -1; // Store the index of the last selected spell
    private int attacksInRow = 0;
    public int attacksInRowMax = 3;


    private void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        animator = GetComponentInParent<Animator>();
        attackTimer = attackCooldown;
        tauntTimer = Random.Range(tauntCooldownMin, tauntCooldownMax);
    }

    public override State RunCurrState()
    {
        if (attacksInRow >= attacksInRowMax) 
        {
            attacksInRow = 0;
            return teleportState;
        }


        Vector3 direction = (playerTransform.position - selfTransform.position).normalized;
        direction.y = 0; // Ensure rotation only around the Y axis
        selfTransform.rotation = Quaternion.LookRotation(direction);

        if (Vector3.Distance(selfTransform.position, playerTransform.position) <= meleeAttackRange)
        {
            attackTimer = attackCooldown;
            return stompState;
        }

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0)
        {
            attackTimer = attackCooldown;

            int newSpellIndex;
            do
            {
                newSpellIndex = Random.Range(0, spellCastSingleState.Length);
            } while (newSpellIndex == lastSpellIndex);

            lastSpellIndex = newSpellIndex;
            attacksInRow++;
            return spellCastSingleState[newSpellIndex];
        }

        tauntTimer -= Time.deltaTime;

        if (tauntTimer <= 0) 
        {
            attacksInRow++;
            tauntTimer = Random.Range(tauntCooldownMin, tauntCooldownMax);

            if (Random.value < 0.5f) {
                return tauntState1;

            }
            else {
                return tauntState2;
            }

                
        }

        return this;
    }

  
}
