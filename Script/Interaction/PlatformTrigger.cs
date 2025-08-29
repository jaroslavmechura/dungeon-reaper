using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : InteractiveObject
{
    public float activeLength;
    public PlatformMover[] platformMovers;

    public MeshRenderer meshRenderer;
    public Material startMat;
    public Material activeMat;

    private int hits = 0;
    private bool isActive;


    private void Start()
    {
        hits = 0;
        SetMiddleMaterial(startMat);
    }

    public override void Interaction()
    {
        if (isActive) { hits = 0; return; }
        hits++;
    }

    private void Update()
    {
        if (hits > 0 && !isActive)
        {
            isActive = true;
            hits = 0;

            foreach (PlatformMover mover in platformMovers)
            {
                mover.Activate();
            }

            SetMiddleMaterial(activeMat);

            StartCoroutine(Deactivate());
        }
    }

    private IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(activeLength);

        foreach (PlatformMover mover in platformMovers)
        {
            mover.Deactivate();
        }

        SetMiddleMaterial(startMat);

        isActive = false;
    }

    // Helper method to change the middle material
    private void SetMiddleMaterial(Material material)
    {
        Material[] materials = meshRenderer.materials;
        if (materials.Length >= 3)
        {
            materials[1] = material;
            materials[2] = material;// Update the middle material (index 1)
            meshRenderer.materials = materials;
        }
    }
}
