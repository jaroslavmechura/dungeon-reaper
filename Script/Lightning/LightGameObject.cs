using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGameObject : MonoBehaviour
{
    [Header("--- References ---")]
    [SerializeField] private Light[] lights;

    [Header("--- Non-Fight Settings ---")]
    [SerializeField] private Color basicLightColor;
    [SerializeField] private float basicLightIntensity;
    [SerializeField] private GameObject[] basicEffects;

    [Header("--- Fight Settings ---")]
    [SerializeField] private Color fightLightColor;
    [SerializeField] private float fightLightIntensity;
    [SerializeField] private GameObject[] fightEffects;

    private void Start()
    {
        
        TurnFriendly();
    }

    public virtual void TurnFriendly() 
    {
        SetLightColor(false);
        SetEffects(ref fightEffects, false);
        SetEffects(ref basicEffects, true);
    }

    public virtual void TurnFight() 
    {
        SetLightColor(true);
        SetEffects(ref basicEffects, false);
        SetEffects(ref fightEffects, true);
    }

    private void SetLightColor(bool isFight) 
    {
        foreach (Light light in lights) { light.color = isFight ? fightLightColor : basicLightColor; light.intensity = isFight ? fightLightIntensity : basicLightIntensity; }
  
    }

    private void SetEffects(ref GameObject[] effectsArray, bool isFight) {
        foreach (GameObject effect in effectsArray) { effect.SetActive(isFight); }
    }
}
