using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("--- Hurt ---")]
    [SerializeField] private AudioSource hurtAudioSource;
    [SerializeField] private AudioClip[] hurtClips;
    private int hurtClipsLen;

    [Header("--- Jump ---")]
    [SerializeField] private AudioSource jumpAudioSource;
    [SerializeField] private AudioClip[] jumpClips;
    private int jumpClipsLen;

    [Header("--- DJump ---")]
    [SerializeField] private AudioSource djumpAudioSource;
    [SerializeField] private AudioClip[] djumpClips;
    private int djumpClipsLen;

    [Header("--- Climb ---")]
    [SerializeField] private AudioSource climbAudioSource;
    [SerializeField] private AudioClip[] climbClips;
    private int climbClipsLen;

    [Header("--- Dash ---")]
    [SerializeField] private AudioSource dashAudioSource;
    [SerializeField] private AudioClip[] dashClips;
    private int dashClipsLen;

    [Header("--- Dash Reffill ---")]
    [SerializeField] private AudioClip[] dashRefillClips;
    private int dashRefillClipsLen;

    [Header("--- PickUp ---")]
    [SerializeField] private AudioSource pickUpAudioSource;
    [SerializeField] private AudioClip healthPickUpClip;
    [SerializeField] private AudioClip armorPickUpClip;
    [SerializeField] private AudioClip ammoPickUpClip;
    [SerializeField] private AudioClip orbPickUpClip;
    [SerializeField] private AudioClip weaponPickUpClip;


    private void Start()
    {
        hurtClipsLen = hurtClips.Length;
        jumpClipsLen = jumpClips.Length;
        climbClipsLen = climbClips.Length;
        dashClipsLen = dashClips.Length;
        djumpClipsLen = djumpClips.Length;
        dashRefillClipsLen = dashRefillClips.Length;
    }

    public void HurtSound() {
        hurtAudioSource.PlayOneShot(hurtClips[Random.Range(0, hurtClipsLen)]);
    }
    public void ClimbSound()
    {
        climbAudioSource.PlayOneShot(climbClips[Random.Range(0, climbClipsLen)]);
    }
    public void JumpSound()
    {
        jumpAudioSource.PlayOneShot(jumpClips[Random.Range(0, jumpClipsLen)]);
    }
    public void DashSound()
    {
        dashAudioSource.PlayOneShot(dashClips[Random.Range(0, dashClipsLen)]);
    }
    public void DJumpSound()
    {
        djumpAudioSource.PlayOneShot(djumpClips[Random.Range(0, djumpClipsLen)]);
    }
    public void DashReffilSound()
    {
        dashAudioSource.PlayOneShot(dashRefillClips[Random.Range(0, dashRefillClipsLen)]);
    }

    public void PickUpSound(PickUpType type) {
        if (type == PickUpType.Health)
        {
            pickUpAudioSource.PlayOneShot(healthPickUpClip);
        }
        else if (type == PickUpType.Armor)
        {
            pickUpAudioSource.PlayOneShot(armorPickUpClip);
        }
        else if (type == PickUpType.RefillAll) {
            pickUpAudioSource.PlayOneShot(orbPickUpClip);
        }
        else if (type == PickUpType.WeaponUnlock)
        {
            pickUpAudioSource.PlayOneShot(weaponPickUpClip);
        }
        else
        {
            pickUpAudioSource.PlayOneShot(ammoPickUpClip);
        }
    }

}
