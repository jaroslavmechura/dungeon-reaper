using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public enum PickUpType {
    Health,
    Armor,
    AmmoPistol,
    AmmoAR,
    AmmoShotgun,
    AmmoRocket,
    RefillAll,
    WeaponUnlock
};

public class PickUp : MonoBehaviour
{
    [SerializeField] private PickUpType type;
    [SerializeField] private int amount;

    [Header("--- Floating ---")]
    [SerializeField] private bool isFloating = false;
    [SerializeField] private float floatSpeed = 2;
     private float floatAmplitude = 0.25f;

    [Header("--- Spin ---")]
    [SerializeField] private bool isRotating = false;
    [SerializeField] private float rotationSpeed = 25;

    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (isFloating)
        {
            float newY = initialPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        if (isRotating)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            if (other.GetComponent<PlayerPickUp>().PickUp(type, amount)) {
                Destroy(gameObject);
            }
            
        }
    }
}
