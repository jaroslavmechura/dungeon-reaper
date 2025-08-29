
using UnityEngine;

public class FightRoomTrigger : InteractiveObject
{
    [Header("--- References ---")]
    [SerializeField] protected RoomManager roomManager;
    private int hits = 0;

    private void Start()
    {
        hits = 0;
    }

    public override void Interaction()
    {
        hits++;
    }

    private void Update()
    {
        if (hits > 0) 
        {
            roomManager.StartRoom();
            Destroy(gameObject);
        }
    }
}
