
using UnityEngine;

public class DoorOpenTrigger : InteractiveObject
{
    [Header("--- Door Reference ---")]
    [SerializeField] private Door door;
    [SerializeField] private bool destroyOnInteraction;
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
            if (!door.isOpen) door.Open();
            else door.Close();

            if (destroyOnInteraction)
            {
                Destroy(gameObject);
            }
            else {
                hits = 0;
            }
        }
    }

}
