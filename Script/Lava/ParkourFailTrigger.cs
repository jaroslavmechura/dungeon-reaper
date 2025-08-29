using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourFailTrigger : MonoBehaviour
{
    public ParkourFailRespawn respawnScript;
    private Collider failCollider;

    public AudioSource impactAudio;
    public AudioClip impactAudioClip;

    private void Start()
    {
        failCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            impactAudio.PlayOneShot(impactAudioClip);
            respawnScript.RespawnPlayer(other.gameObject);
            failCollider.enabled = false;
            StartCoroutine(ResetCollider());
        }
    }

    private IEnumerator ResetCollider() {
        yield return new WaitForSeconds(1f);
        failCollider.enabled = true;
    }
}
