using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchLightGameObject : LightGameObject
{
    [Header("--- Torch Specific Settings ---")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material basicMaterial;
    [SerializeField] private Material fightMaterial;

    private void Start()
    {
        TurnFriendly();
    }

    public override void TurnFriendly()
    {
        base.TurnFriendly();
        SetMaterial(false);
    }

    public override void TurnFight()
    {
        base.TurnFight();
        SetMaterial(true);
    }

    private void SetMaterial(bool isFight)
    {
        Material[] materials = meshRenderer.materials;
        materials[1] = isFight ? fightMaterial : basicMaterial;
        meshRenderer.materials = materials;
    }
}
