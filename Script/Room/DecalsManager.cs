using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalsManager : MonoBehaviour
{
    [SerializeField] private GameObject[] decalsObjects;
    [SerializeField] private Color pentagramOffFightColor;
    [SerializeField] private Color pentagramInFightColor;

    private static readonly int BloodColorID = Shader.PropertyToID("_BloodColor"); // Assuming the shader property name is "_BloodColor"

    public void Start()
    {
        ChangeDecalColors(pentagramOffFightColor);
    }

    public void StartFight()
    {
        ChangeDecalColors(pentagramInFightColor);
    }

    public void EndFight()
    {
        ChangeDecalColors(pentagramOffFightColor);
    }

    private void ChangeDecalColors(Color color)
    {
        foreach (GameObject decalObject in decalsObjects)
        {
            var decalProjector = decalObject.GetComponent<DecalProjector>(); // Assuming you're using the Decal Projector component
            if (decalProjector != null)
            {
                var material = decalProjector.material;
                if (material != null)
                {
                    material.SetColor(BloodColorID, color);
                }
            }
        }
    }
}
