using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ParkourFailRespawn : MonoBehaviour
{
    public float damage;

    private float length;

    //--- Player Reffs ---
    private GameObject playerObj;
    private Transform playerTransform;
    private PlayerInput playerInput;
    private PlayerHealth playerHealth;
    private PlayerController playerController;
    private Rigidbody rb;

    [SerializeField] private PlayerDamage damageImpactType;

    private void Start()
    {
        playerObj = GameObject.FindWithTag("Player");
        playerTransform = playerObj.transform;
        playerInput = playerObj.GetComponent<PlayerInput>();
        playerHealth = playerObj.GetComponent<PlayerHealth>();
        playerController = playerObj.GetComponent<PlayerController>();
        rb = playerObj.GetComponent<Rigidbody>();
    }

    public void RespawnPlayer(GameObject player) {
        length = playerController.length;
        StartCoroutine(Respawn(player));
    }

    private IEnumerator Respawn(GameObject player) {
        playerHealth.TakeDamage(damage, damageImpactType);
        playerController.FadeOut(length / 4f);
        playerInput.enabled = false;
        rb.isKinematic = true;

        yield return new WaitForSeconds(length / 4f);

        playerTransform.position = transform.position;
        playerTransform.rotation = transform.rotation;

        yield return new WaitForSeconds((length / 4f) * 3f);

        rb.isKinematic = false;
        playerInput.enabled = true;
        playerController.FadeIn((length / 4f) * 3f);
    }
}
