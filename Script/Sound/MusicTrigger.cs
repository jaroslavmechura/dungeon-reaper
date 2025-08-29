using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [SerializeField] private bool nextNonCombat;

    private RoomMusicManager roomMusicManager;

    private void Start()
    {
        roomMusicManager = FindObjectOfType<RoomMusicManager>();
        if (roomMusicManager == null) Debug.LogError("ref not found");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (nextNonCombat) 
            {
                roomMusicManager.JustNextNonCombatClip();
            }
            else 
            {
                roomMusicManager.JustNextCombatClip();
            }
             
            Destroy(gameObject);
        }
    }
}
