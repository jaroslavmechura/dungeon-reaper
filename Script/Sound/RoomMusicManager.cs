using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMusicManager : MonoBehaviour
{
    [Header("--- References ---")]
    [SerializeField] private AudioSource outsideCombat;
    [SerializeField] private AudioSource inCombat;

    [Header("--- Clips ---")]
    [SerializeField] private AudioClip[] outsideCombatClip;
    [SerializeField] private AudioClip[] inCombatClip;


    [SerializeField] private int peaceClipIndex;
    [SerializeField]private int fightClipIndex;

    private void Start()
    {
        fightClipIndex = 0;
        peaceClipIndex = 0;

        outsideCombat.clip = outsideCombatClip[peaceClipIndex];
        inCombat.clip = inCombatClip[fightClipIndex];

        outsideCombat.Play();
    }

    public void StartFight() {
        inCombat.Play();
        outsideCombat.Stop();

        peaceClipIndex++;
        outsideCombat.clip = outsideCombatClip[peaceClipIndex];
    }

    public void EndFight() {
        outsideCombat.Play();
        inCombat.Stop();

        fightClipIndex++;
        if (fightClipIndex >= inCombatClip.Length) fightClipIndex = 0;
        inCombat.clip = inCombatClip[fightClipIndex];
    }

    public void JustNextNonCombatClip() 
    {
        peaceClipIndex++;
        outsideCombat.clip = outsideCombatClip[peaceClipIndex];

        outsideCombat.Play();
        inCombat.Stop();
    }

    public void JustNextCombatClip()
    {
        fightClipIndex++;
        inCombat.clip = inCombatClip[fightClipIndex];

        inCombat.Play();
        outsideCombat.Stop();
    }
}
