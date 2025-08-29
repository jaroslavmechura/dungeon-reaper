using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerWeapons : MonoBehaviour
{
    [Header("--- Weapons ---")]
    [SerializeField] public Weapon[] weapons;
    [SerializeField] private Weapon currWeapon;
    [SerializeField] private int currWeaponId;

    [Header("--- Weapon Switch ---")]
    [SerializeField] private float weaponSwitchLength;
    [SerializeField] private bool isSwitching;

    [Header("--- Weapon Unlock ---")]
    [SerializeField] private List<bool> weaponUnlocked;

    // --- References ---
    private PlayerMovement playerMovement;
    private PlayerInput playerInput;
    private Animator currWeaponAnimator;

    private bool isClimbingAnimation = false;

    public GameObject NoAmmoText;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();

        currWeaponId = 0;
        currWeapon = weapons[currWeaponId];
        currWeaponAnimator = currWeapon.GetComponent<Animator>();

        weaponUnlocked = new List<bool>();

        foreach (Weapon weapon in weapons)
        {
            weapon.Activate(false);
            weaponUnlocked.Add(false);
        }

        weaponUnlocked[currWeaponId] = true;

        StartCoroutine(EquipWeapon());
    }

    public void HandleWeapons() 
    {
        HandleWeaponSwitch();
        HandleWeaponAction();
        HandleWeaponAnimation();

        NoAmmoText.SetActive(currWeapon.currAmmo <= 0);
    }

    private void HandleWeaponSwitch() 
    {
        if (playerInput.weaponId != currWeaponId && !isSwitching)
        {
            if (weaponUnlocked[playerInput.weaponId])
            {
                currWeaponId = playerInput.weaponId;
                StartCoroutine(EquipWeapon());
            }
            else 
            {
                playerInput.weaponId = currWeaponId;
                playerInput.newWeaponId = currWeaponId;
            }
        }
    }

    private IEnumerator EquipWeapon() 
    {
        isSwitching = true;
        currWeaponAnimator.SetTrigger("DeEquip");

        yield return new WaitForSeconds(weaponSwitchLength / 2f);

        currWeapon.Activate(false);
        currWeapon = weapons[currWeaponId];
        currWeapon.Activate(true);

        currWeaponAnimator = currWeapon.GetComponent<Animator>();
        currWeaponAnimator.SetTrigger("Equip");

        yield return new WaitForSeconds(weaponSwitchLength / 2f);

        isSwitching = false;
    }

    private void HandleWeaponAction()
    {
        if (playerInput.basicShot && !playerMovement.isClimbing && !isSwitching) currWeapon.BasicShot();
    }
    private void HandleWeaponAnimation() 
    {
        if (!playerMovement.isClimbing && isClimbingAnimation) isClimbingAnimation = false;

        currWeaponAnimator.SetBool("isMoving", playerMovement.isMoving);
        if (playerMovement.isClimbing && !isClimbingAnimation)
        {
            currWeaponAnimator.SetTrigger("Climb");
            isClimbingAnimation = true;
        } 
    }

    public bool WeaponPickUp(int unlockID) 
    {
        if (weaponUnlocked[unlockID]) return false;

        weaponUnlocked[unlockID] = true;

        currWeaponId = unlockID;
        playerInput.weaponId = unlockID;
        playerInput.newWeaponId = unlockID;

        StartCoroutine(EquipWeapon());
        return true;
    }
}
