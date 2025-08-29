using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHit : MonoBehaviour
{
    public State state;



    private void OnCollisionEnter(Collision collision)
    {
        

        if (collision.collider.CompareTag("Player")) {
            state.DealDamage();
            GetComponent<Collider>().enabled = false;
        }
    }
}
