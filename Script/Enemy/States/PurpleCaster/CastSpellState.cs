using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastSpellState : State
{
    // --- Next State ---
    public State chaseState;

    [Header("--- Attack Settings ---")]
    [SerializeField] private GameObject spellObject;
    [SerializeField] private float beforeHitTime;
    [SerializeField] private float afterHitTime;

    [Header("--- References ---")]
    [SerializeField] private Transform selfTransform;
    [SerializeField] private Transform spellSpawnTransform;

    private Animator animator;
    private bool isAttackingStart = false;
    private bool isAttackingEnd = false;

    private Transform playerTransform;

    // List to cache created spells
    private List<Spell> cachedSpells = new List<Spell>();

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    public override State RunCurrState()
    {
        if (isAttackingEnd)
        {
            isAttackingStart = false;
            isAttackingEnd = false;
            return chaseState;
        }

        if (!isAttackingStart) StartCoroutine(Attack());

        return this;
    }

    private IEnumerator Attack()
    {
        isAttackingStart = true;
        animator.SetTrigger("CastStart");

        // --- init projectile
        GameObject spawnedSpellObj = Instantiate(spellObject, spellSpawnTransform.position, Quaternion.identity, null);
        Spell spell = spawnedSpellObj.GetComponent<Spell>();
        spell.CreateSpell(beforeHitTime);
        cachedSpells.Add(spell); // Cache the created spell

        // --- rotate to player before releasing projectile
        float elapsedTime = 0f;
        Quaternion startingRotation = selfTransform.rotation;

        Vector3 directionToPlayer = playerTransform.position - selfTransform.position;
        directionToPlayer.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        while (elapsedTime < beforeHitTime)
        {
            selfTransform.rotation = Quaternion.Slerp(startingRotation, targetRotation, elapsedTime / beforeHitTime);
            elapsedTime += Time.deltaTime;

            spawnedSpellObj.transform.position = spellSpawnTransform.position;

            yield return null;
        }

        animator.SetTrigger("CastEnd");

        // --- release projectile
        spell.UseSpell();
        cachedSpells.Remove(spell); // Remove from cache once used

        // --- rotate to player after releasing projectile
        elapsedTime = 0f;
        startingRotation = selfTransform.rotation;

        directionToPlayer = playerTransform.position - selfTransform.position;
        directionToPlayer.y = 0;
        targetRotation = Quaternion.LookRotation(directionToPlayer);

        while (elapsedTime < afterHitTime)
        {
            selfTransform.rotation = Quaternion.Slerp(startingRotation, targetRotation, elapsedTime / afterHitTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        selfTransform.rotation = targetRotation;

        isAttackingEnd = true;
    }

    // Public method to destroy cached spells
    public override void OnDeath()
    {
        foreach (Spell spell in cachedSpells)
        {
            if (spell != null)
            {
                Destroy(spell.gameObject);
            }
        }
        cachedSpells.Clear();
    }
}
