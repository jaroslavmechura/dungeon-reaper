using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  abstract class State : MonoBehaviour
{
    public abstract State RunCurrState();
    public virtual void OnDeath() { }

    public virtual void DealDamage() { }
}
