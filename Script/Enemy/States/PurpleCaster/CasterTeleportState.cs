using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CasterTeleportState : State
{
    public State idleState;

    public Transform[] teleportPoints;
    public VisualEffect teleportEffect;

    public GameObject[] meshRenderers;

    private bool isStarted = false;
    private bool isEnded = false;

    private Animator animator;

    private Transform playerTransform;
    public Transform selfTransform;
    private EnemyAudio enemyAudio;
    private void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        animator = GetComponentInParent<Animator>();
        enemyAudio = GetComponentInParent<EnemyAudio>();
    }

    public override State RunCurrState()
    {
        if (!isStarted)
        {
            isStarted = true;
            StartCoroutine(TeleportCoroutine());
        }

        if (isEnded)
        {
            isStarted = false;
            isEnded = false;
            return idleState;
        }

        return this;
    }

    private IEnumerator TeleportCoroutine()
    {
        // Play VFX effect and disable mesh renderers
        teleportEffect.Play();
        animator.SetTrigger("TeleportEnter");
        enemyAudio.EffectSound();

        // Wait for the VFX effect duration
        yield return new WaitForSeconds(teleportEffect.GetFloat("Duration") / 2);

        SetMeshRenderersActive(false);

        yield return new WaitForSeconds(teleportEffect.GetFloat("Duration") / 2);

        // Teleport to the furthest point from the player
        Transform furthestPoint = GetFurthestTeleportPoint();
        selfTransform.position = furthestPoint.position;

        // Play VFX effect again and enable mesh renderers
        teleportEffect.Play();
        animator.SetTrigger("TeleportExit");

        // Wait for the VFX effect duration again
        yield return new WaitForSeconds(teleportEffect.GetFloat("Duration") / 2);

        SetMeshRenderersActive(true);

        yield return new WaitForSeconds(teleportEffect.GetFloat("Duration") / 2);

        isEnded = true;
    }

    private void SetMeshRenderersActive(bool active)
    {
        foreach (var renderer in meshRenderers)
        {
            renderer.SetActive(active);
        }
    }

    private Transform GetFurthestTeleportPoint()
    {
        Transform furthestPoint = null;
        float maxDistance = float.MinValue;

        foreach (var point in teleportPoints)
        {
            float distance = Vector3.Distance(playerTransform.position, point.position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthestPoint = point;
            }
        }

        return furthestPoint;
    }
}
