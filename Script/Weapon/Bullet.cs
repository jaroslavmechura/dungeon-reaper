using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public ParticleSystem particleSystemBullet;

    public void TurnOff() {
        particleSystemBullet.Stop();
        Destroy(gameObject, 0.25f);
    }
}
