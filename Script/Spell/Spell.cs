using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : MonoBehaviour
{
    public abstract void CreateSpell(float duration);
    public abstract void UseSpell();
}
