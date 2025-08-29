using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public float spawndelay = 0f;
    [Header("--- References ---")]
    [SerializeField] private FightManager fightManager;
    [SerializeField] private RoomMusicManager roomMusicManager;
    [SerializeField] private RoomLightObjectManager roomLightObjectManager;
    [SerializeField] private ProgressObstacle[] progressObstaclesToClose;
    [SerializeField] private ProgressObstacle[] progressObstaclesToOpen;
    [SerializeField] private DecalsManager decalsManager;

    [Header("--- Specials ---")]
    [SerializeField] private PlatformMover[] platforms;

    public void StartRoom() {
       
        roomMusicManager.StartFight();
        roomLightObjectManager.RoomLightFight();
        decalsManager.StartFight();

 
        foreach (ProgressObstacle progressObstacle in progressObstaclesToClose) { progressObstacle.Close(); }

        foreach (PlatformMover mover in platforms) { mover.Deactivate(); mover.ActiveMaterial(); }

        if (spawndelay == 0f)
        {
            fightManager.StartFight(this);
        }
        else {
            StartCoroutine(StartWithDelay());
        }
    }

    public void EndRoom() { 
        roomMusicManager.EndFight();
        roomLightObjectManager.RoomLightFriendly();
        decalsManager.EndFight();

        foreach (ProgressObstacle progressObstacle in progressObstaclesToOpen) { progressObstacle.Open(); }
        
        foreach(PlatformMover mover in platforms) { mover.DeactivateMaterial(); }
    }

    private IEnumerator StartWithDelay() {
        yield return new WaitForSeconds(spawndelay);
        fightManager.StartFight(this);
    }
}
