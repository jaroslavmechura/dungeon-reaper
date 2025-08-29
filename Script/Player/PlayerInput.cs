using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("--- Mouse Settings ---")]
    [SerializeField] private float mouseSensitivity = 3f;

    // --- Basic Movement ---
    public Vector2 mouseInput { get; private set; }
    public float moveX { get; private set; }
    public float moveZ { get; private set; }

    // --- Movement Skills ---
    public bool jumpInput { get; private set; }
    public bool dashInput { get; private set; }
    public Vector3 dashDirection { get; private set; }

    // --- Weapon ---
    public int weaponId;
    public bool basicShot { get; private set; }
    [SerializeField] private int totalWeapons = 3; // The total number of weapons
    private float scrollInput;
    public int newWeaponId;
    private Coroutine scrollCoroutine;
    [SerializeField] private float scrollDelay = 0.2f; // Delay before applying the new weapon ID

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        weaponId = 0;
        newWeaponId = weaponId;
    }

    public void HandleInput()
    {
        HandleMouseInput();
        HandleMovementInput();
        HandleActionInput();
        HandleDashDirection();
        HandleWeapon();
        HandleInventory();
    }

    private void HandleMouseInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        mouseInput = new Vector2(mouseX, mouseY);
    }

    private void HandleMovementInput()
    {
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");
    }

    private void HandleActionInput()
    {
        jumpInput = Input.GetKeyDown(KeyCode.Space);
        dashInput = Input.GetKeyDown(KeyCode.LeftShift);
    }

    private void HandleDashDirection()
    {
        dashDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) { dashDirection += transform.forward; }
        if (Input.GetKey(KeyCode.S)) { dashDirection -= transform.forward; }
        if (Input.GetKey(KeyCode.D)) { dashDirection += transform.right; }
        if (Input.GetKey(KeyCode.A)) { dashDirection -= transform.right; }
    }

    private void HandleWeapon()
    {
        basicShot = Input.GetMouseButton(0);
    }

    private void HandleInventory()
    {
        if (Input.GetKey(KeyCode.Alpha1)) { weaponId = 0; newWeaponId = 0; return; }
        if (Input.GetKey(KeyCode.Alpha2)) { weaponId = 1; newWeaponId = 1; return; }
        if (Input.GetKey(KeyCode.Alpha3)) { weaponId = 2; newWeaponId = 2; return; }
        if (Input.GetKey(KeyCode.Alpha4)) { weaponId = 3; newWeaponId = 3; return; }

        scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0f)
        {
            // Determine direction of scroll (up or down)
            int scrollDirection = (scrollInput > 0f) ? 1 : -1;

            // Calculate the new weapon id
            newWeaponId = newWeaponId + scrollDirection;

            // Wrap around if exceeding weapon count boundaries
            if (newWeaponId >= totalWeapons)
            {
                newWeaponId = 0; // Cycle back to the first weapon
            }
            else if (newWeaponId < 0)
            {
                newWeaponId = totalWeapons - 1; // Cycle back to the last weapon
            }

            // Restart the coroutine for applying the weapon change after delay
            if (scrollCoroutine != null)
            {
                StopCoroutine(scrollCoroutine);
            }
            scrollCoroutine = StartCoroutine(ApplyWeaponChangeAfterDelay());
        }
        
    }

    private IEnumerator ApplyWeaponChangeAfterDelay()
    {
        yield return new WaitForSeconds(scrollDelay);
        weaponId = newWeaponId;
    }
}
