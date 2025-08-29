using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLightObjectManager : MonoBehaviour
{
    [SerializeField] private LightGameObject[] lightObjects;
    [SerializeField] private bool isFight;

    private bool isReallyFight;

    private void Start()
    {
        foreach (LightGameObject lightObject in lightObjects) lightObject.TurnFriendly();
        isFight = false;
        isReallyFight = false;
    }

    private void Update()
    {
        if (isFight && !isReallyFight)
        {
            RoomLightFight();
            isReallyFight = true;
        }
        else if (!isFight && isReallyFight) 
        {
            RoomLightFriendly();
            isReallyFight = false;
        }
    }

    public void RoomLightFriendly() 
    {
        foreach (LightGameObject lightObject in lightObjects) lightObject.TurnFriendly();
    }

    public void RoomLightFight() 
    {
        foreach (LightGameObject lightObject in lightObjects) lightObject.TurnFight();
    }
}
