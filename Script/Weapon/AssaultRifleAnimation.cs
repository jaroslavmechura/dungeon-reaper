using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssaultRifleAnimation : WeaponAnimation
{
    [SerializeField] private Transform head;
    [SerializeField] private Transform mag;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float minSpeed;
    [SerializeField] private float currSpeed; 
    [SerializeField] private float shotSpeedAdd;
    [SerializeField] private float shotSpeedDropoff;


    private void Update()
    {
        // Rotate head around its local x-axis
        head.Rotate(Vector3.right * currSpeed * Time.deltaTime);

        // Rotate mag around its local z-axis
        mag.Rotate(Vector3.forward * currSpeed * Time.deltaTime);

        // Decrease speed by shotSpeedDropoff each frame
        currSpeed -= shotSpeedDropoff * Time.deltaTime;

        // Clamp the speed between minSpeed and maxSpeed
        currSpeed = Mathf.Clamp(currSpeed, minSpeed, maxSpeed);
    }

    public override void AddShotSpeed()
    {
        // Increase speed by shotSpeedAdd
        currSpeed += shotSpeedAdd;

        // Clamp the speed between minSpeed and maxSpeed
        currSpeed = Mathf.Clamp(currSpeed, minSpeed, maxSpeed);
    }
}
