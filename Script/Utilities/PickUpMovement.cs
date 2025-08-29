using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpMovement : MonoBehaviour
{
    [Header("--- Float Movement ---")]
    [SerializeField] private float floatUp = 1f;
    [SerializeField] private float floatDown = -2f;
    [SerializeField] private float floatSpeed = 3f;

    [Header("--- Rotation ---")]
    [SerializeField] private float rotationSpeed = 90f;

    private Vector3 startLocalPosition;
    private float timer;

    private void Start()
    {
        startLocalPosition = transform.localPosition;
        timer = 0f;
    }

    private void Update()
    {
        HandleFloating();
        HandleRotation();
    }

    private void HandleFloating()
    {
        float range = floatUp - floatDown;
        float offset = Mathf.Sin(Time.time * (Mathf.PI * 2f / floatSpeed)) * (range / 2f);
        float middleY = (floatUp + floatDown) / 2f;

        Vector3 localPos = startLocalPosition;
        localPos.y = startLocalPosition.y + middleY + offset;
        transform.localPosition = localPos;
    }

    private void HandleRotation()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
