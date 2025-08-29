using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUp : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private PlayerWeapons playerWeapons;
    private PlayerAudio playerAudio;
    private Weapon[] weapons;

    private void Start()
    {
        playerAudio = GetComponent<PlayerAudio>();
        playerHealth = GetComponent<PlayerHealth>();
        playerWeapons = GetComponent<PlayerWeapons>();
        weapons = playerWeapons.weapons;
    }

    public bool PickUp(PickUpType type, int amount) {
        if (type == PickUpType.Health)
        {
            if (!playerHealth.AddHealth(amount)) return false;
        }
        else if (type == PickUpType.Armor)
        {
            if (!playerHealth.AddArmor(amount)) return false;
        }
        else if (type == PickUpType.AmmoPistol)
        {
            if (!weapons[0].AddAmmo(amount)) return false;
        }
        else if (type == PickUpType.AmmoAR)
        {
            if (!weapons[1].AddAmmo(amount)) return false;
        }
        else if (type == PickUpType.AmmoShotgun)
        {
            if (!weapons[2].AddAmmo(amount)) return false;
        }
        else if (type == PickUpType.AmmoRocket)
        {
            if (!weapons[3].AddAmmo(amount)) return false;
        }
        else if (type == PickUpType.RefillAll)
        {
            bool anySuccess = false;
            anySuccess |= playerHealth.AddHealth(amount);
            anySuccess |= playerHealth.AddArmor(amount);
            anySuccess |= weapons[0].AddAmmo(amount);
            anySuccess |= weapons[1].AddAmmo(amount);
            anySuccess |= weapons[2].AddAmmo(amount);
            anySuccess |= weapons[3].AddAmmo(amount);

            if (!anySuccess) return false;
        }
        else if (type == PickUpType.WeaponUnlock) 
        {
            if (!playerWeapons.WeaponPickUp(amount)) return false;
        }

        playerAudio.PickUpSound(type);
        return true;
    }
}
