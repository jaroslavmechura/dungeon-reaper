using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitPlayer : MonoBehaviour
{
    public StateManager stateManager;



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player")) {
            stateManager.currState.DealDamage();
            GetComponent<Collider>().enabled = false;
        }
    }
}
