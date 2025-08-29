using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRemoveAmmoAndHealth : InteractiveObject
{
    private PlayerWeapons weapons;
    private PlayerHealth health;


    private void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        weapons = p.GetComponent<PlayerWeapons>();
        health = p.GetComponent<PlayerHealth>();
    }

    public override void Interaction()
    {
        health.currArmor = 0;

        foreach (Weapon weap in weapons.weapons)
        {
            weap.currAmmo = 0;
        }

        StartCoroutine(ReduceHealthOverTime(health.maxHealth, 1, 5f));
    }

    private IEnumerator ReduceHealthOverTime(float startHealth, float endHealth, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            health.currHealth = Mathf.Lerp(startHealth, endHealth, elapsed / duration);
            elapsed += Time.deltaTime;
            health.HandleHealth(); // Ensure the health UI and effects are updated
            yield return null;
        }

        health.currHealth = endHealth;
        health.HandleHealth(); // Final update to ensure the health is set to the exact end value
    }
}
