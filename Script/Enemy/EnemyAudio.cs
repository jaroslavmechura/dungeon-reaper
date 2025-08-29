using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    [Header("--- Hurt ---")]
    [SerializeField] private AudioSource hurtAudioSource;
    [SerializeField] private AudioClip[] hurtClips;
    private int hurtClipsLen;

    [Header("--- MeleeAttack ---")]
    [SerializeField] private AudioSource meleeAttackAudioSource;
    [SerializeField] private AudioClip[] meleeAttackClips;
    private int meleeAttackClipsLen;

    [Header("--- Effects ---")]
    [SerializeField] private AudioSource effectsAudioSource;
    [SerializeField] private AudioClip[] effectsClips;
    [SerializeField] private float effectsTimerMax;
    [SerializeField] private float effectsTimerMin;
    private int effectsClipsLen;
    private float effectsTimer;

    [Header("--- Specials ---")]
    [SerializeField] private AudioClip[] screamClips;
    private int screamClipsLen;
   

    private void Start()
    {
        hurtClipsLen = hurtClips.Length;
        meleeAttackClipsLen = meleeAttackClips.Length;
        effectsClipsLen = effectsClips.Length;
        screamClipsLen = screamClips.Length;

        effectsTimer = Random.Range(effectsTimerMin, effectsTimerMax);
    }

    private void Update()
    {
        effectsTimer -= Time.deltaTime * 1f;
        if (effectsTimer <= 0f) {
            EffectSound();
            effectsTimer = Random.Range(effectsTimerMin, effectsTimerMax);
        }
    }
    public void EffectSound()
    {
       effectsAudioSource.PlayOneShot(effectsClips[Random.Range(0, effectsClipsLen - 1)]);
    }

    public void HurtSound()
    {
        hurtAudioSource.PlayOneShot(hurtClips[Random.Range(0, hurtClipsLen - 1)]);
    }

    public void MeleeAttackSound() 
    {
        meleeAttackAudioSource.PlayOneShot(meleeAttackClips[Random.Range(0, meleeAttackClipsLen - 1)]);
    }

    public void ScreamSound() {
        if (screamClipsLen == 0) return;
        effectsAudioSource.PlayOneShot(screamClips[Random.Range(0, screamClipsLen - 1 )]);
    }

}
