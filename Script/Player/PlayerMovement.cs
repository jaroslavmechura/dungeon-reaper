using EZCameraShake;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("--- Movement Settings ---")]
    [SerializeField] private float speed = 8f;

    [Header("--- Jump Settings ---")]
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float doubleJumpForce = 12f;
    [SerializeField] private float groundCheckSphereRadius = 0.45f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask climbLayer;
    [SerializeField] private LayerMask stepLayer;

    [Header("--- Dash Settings ---")]
    [SerializeField] private float dashForce = 35f;
    [SerializeField] private float dashDuration = 0.2f;

    [SerializeField] private int dashCharges = 2;
    [SerializeField] private float dashRefillScale;
    [SerializeField] private float dashCooldownTimer;
    [SerializeField] private float dashCooldownTimerMax;
    [SerializeField] private float dashCooldownReffilOffsetTimer;
    [SerializeField] private float dashCooldownReffilOffsetTimerMax;
    [SerializeField] private bool dashRefillSound1 = false;
    [SerializeField] private bool dashRefillSound2 = false;

    [Header("--- States ---")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool canDoubleJump;
    [SerializeField] private bool isDashing;
    [SerializeField] public bool isClimbing;   
    [SerializeField] private bool isStepping;
    public bool isMoving;

    [Header("--- Debug Options ---")]
    [SerializeField] private bool drawGroundCheck;
    [SerializeField] private bool drawWallStuckCheck;
    [SerializeField] private bool drawWallClimbCheck;
    [SerializeField] private bool drawWallStepCheck;

    [Header("--- UI ---")]
    [SerializeField] private Slider dashCountUI;
    [SerializeField] private Image canJumpUI;
    [SerializeField] private Image canDoubleJumpUI;


    // --- References ---
    private Rigidbody rb;
    private PlayerInput input;
    private PlayerAudio playerAudio;
    private Transform playerCamera; 
    
    // --- Look Around ---
    private float xRotation = 0f;

    // --- Dash ---
    private float dashTime;

    // --- Body Parts ---
    private Transform headTransform;
    private Transform torsoTransform;
    private Transform legsTransform;

    // --- Wall Stuck ---
    private bool isStuckForward;
    private bool isStuckBack;
    private bool isStuckRight;
    private bool isStuckLeft;

    private bool isStuckForwardL;
    private bool isStuckForwardR;
    private bool isStuckBackL; 
    private bool isStuckBackR;

    private Vector3 inputVelocity;
    private float moveX;
    private float moveZ;
    private float wallStuckCheckLen = 0.55f; 

    // --- Wall Climb ---
    private bool isWallClimbForwardLow;
    private bool isWallClimbForwardCamera;
    private bool isWallClimbHitFound;
    private float wallClimbCheckLen = 0.75f;
    private float wallClimbDuration = 0.3f;
    private float wallClimbTimeStart;
    private Vector3 wallClimbFinalPosition;
    private Vector3 wallClimbStartPosition;
    private Vector3[] wallClimbDirections;

    // --- Wall Steps ---
    private Vector3[] wallStepDirections;
    private bool isWallStepLow;
    private bool isWallStepMid;
    private float wallStepDuration = 0.15f;

    private Vector3 tempDir;

    // --- Debug Draw ---
    private Vector3 debugDrawDir;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<PlayerInput>();
        playerCamera = Camera.main.transform;
        playerAudio = GetComponent<PlayerAudio>();

        headTransform = transform.Find("Head");
        torsoTransform = transform.Find("Torso");
        legsTransform = transform.Find("Legs");
        if (headTransform == null) Debug.Log("Head not found.");
        if (torsoTransform == null) Debug.Log("Torso not found.");
        if (legsTransform == null) Debug.Log("Legs not found.");

        wallStepDirections = new Vector3[]
        {
            transform.forward,
            transform.right,
             -transform.forward,
             -transform.right,
            (transform.forward + transform.right).normalized,
            (transform.right - transform.forward).normalized,
            (-transform.forward - transform.right).normalized,
            (-transform.right + transform.forward).normalized
        };

        wallClimbDirections = new Vector3[]
        {
            transform.forward,/*
            transform.forward + transform.right / 2,
            transform.forward - transform.right / 2,*/
        };

        dashCooldownTimer = dashCooldownTimerMax;

        dashCountUI.minValue = 0;
        dashCountUI.maxValue = dashCooldownTimerMax;
        dashCountUI.value = dashCooldownTimer;
    }

    public void HandleMovement()
    {
        isMoving = false;

        HandleCooldowns();
        HandleUI();

        if (isClimbing)
        {
            print("isClimbing");
            HandleClimb(wallClimbDuration);
            return;
        }

        if (isStepping)
        {
            print("isStepping");
            HandleClimb(wallStepDuration); 
            HandleMouseRotation(1);
            return;
        }

        if (isDashing)
        {
            HandleDash();
            HandleMouseRotation(2);
            return;
        }

        HandleBasicMovement();
        HandleJump();
        HandleDash();
        HandleMouseRotation(1);
        

        CheckStep();
        if (isStepping) return;
        CheckClimb();

        CheckMoving();
    }

    private void HandleBasicMovement()
    {
        isGrounded = Physics.CheckSphere(legsTransform.position, groundCheckSphereRadius, groundLayer);

        if (isGrounded)
        {
            canDoubleJump = true;
        }

        WallStuckStatus();
        ModifiedMovementInput();

        rb.velocity = new Vector3(inputVelocity.x, rb.velocity.y, inputVelocity.z);

        if (!isGrounded)
        {
            rb.velocity += new Vector3(0, Physics.gravity.y * Time.deltaTime, 0);
        }
    }

    private void WallStuckStatus() {
        isStuckForward = Physics.Raycast(torsoTransform.position, transform.forward, wallStuckCheckLen, climbLayer);
        isStuckBack = Physics.Raycast(torsoTransform.position, -transform.forward, wallStuckCheckLen, climbLayer);
        isStuckRight = Physics.Raycast(torsoTransform.position, transform.right, wallStuckCheckLen, climbLayer);
        isStuckLeft = Physics.Raycast(torsoTransform.position, -transform.right, wallStuckCheckLen, climbLayer);

        isStuckForwardR = Physics.Raycast(torsoTransform.position, (transform.forward + transform.right).normalized, wallStuckCheckLen, climbLayer);
        isStuckForwardL = Physics.Raycast(torsoTransform.position, (transform.forward - transform.right).normalized, wallStuckCheckLen, climbLayer);

        isStuckBackR = Physics.Raycast(torsoTransform.position, (-transform.forward + transform.right).normalized, wallStuckCheckLen, climbLayer);
        isStuckBackL = Physics.Raycast(torsoTransform.position, (-transform.forward - transform.right).normalized, wallStuckCheckLen, climbLayer);
    }

    private void ModifiedMovementInput() {
        moveX = input.moveX;
        moveZ = input.moveZ;

        if (isStuckForward && moveZ > 0) moveZ = 0;
        if (isStuckBack && moveZ < 0) moveZ = 0;
        if ((isStuckRight || isStuckForwardR || isStuckBackR) && moveX > 0) moveX = 0;
        if ((isStuckLeft || isStuckForwardL || isStuckBackL) && moveX < 0) moveX = 0;

        if (!isGrounded && (isStuckForward || isStuckForwardL || isStuckForwardR || isStuckBack || isStuckBackL || isStuckBackR || isStuckLeft || isStuckRight))
        {
            moveX = 0;
            moveZ = 0;
        }

        Vector2 tempVec2 = new Vector2(moveX, moveZ);

        if (tempVec2.magnitude > 1) tempVec2.Normalize();

        inputVelocity = transform.right * tempVec2.x + transform.forward * tempVec2.y;

        

        inputVelocity *= speed;
    }


    private void HandleMouseRotation(int ratio)
    {
        float mouseX = input.mouseInput.x / ratio;
        float mouseY = input.mouseInput.y / ratio;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleJump()
    {
        if (input.jumpInput)
        {
            if (isGrounded)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
                playerAudio.JumpSound();
            }
            else if (canDoubleJump)
            {
                rb.velocity = new Vector3(rb.velocity.x, doubleJumpForce, rb.velocity.z);
                canDoubleJump = false;
                playerAudio.DJumpSound();
            }
        }
    }

    private void HandleDash()
    {
        if (input.dashInput && dashCooldownTimer >= dashCooldownTimerMax/dashCharges)
        {
            isDashing = true;
            dashTime = Time.time + dashDuration;
            rb.velocity = input.dashDirection.normalized * dashForce;
            playerAudio.DashSound();

            CameraShaker.Instance.ShakeOnce(3, 3, 0.1f, 0.75f);

            dashCooldownTimer -= dashCooldownTimerMax / dashCharges;
            dashCooldownReffilOffsetTimer += dashCooldownReffilOffsetTimerMax;

            if (dashCooldownTimer >= (dashCooldownTimerMax / dashCharges)) {
                dashRefillSound1 = false;
            }
            if (dashCooldownTimer < (dashCooldownTimerMax / dashCharges)) {
                dashRefillSound2 = false;
            }
        }

        if (isDashing && Time.time >= dashTime)
        {
            isDashing = false;
        }
    }

    private void CheckClimb()
    {
        if (isGrounded) return;

        foreach (Vector3 dir in wallClimbDirections) 
        {
            RaycastHit lowHit;

            tempDir = legsTransform.TransformDirection(dir);
            isWallClimbForwardLow = Physics.Raycast(torsoTransform.position, tempDir, out lowHit, wallClimbCheckLen, climbLayer);
            tempDir = playerCamera.TransformDirection(dir);
            isWallClimbForwardCamera = Physics.Raycast(playerCamera.position, tempDir + playerCamera.TransformDirection(transform.up) / 4, wallClimbCheckLen * 2, climbLayer);

            if (isWallClimbForwardLow && !isWallClimbForwardCamera)
            {
                RaycastHit hit;

                isWallClimbHitFound = Physics.Raycast((playerCamera.position + ((tempDir + playerCamera.TransformDirection(transform.up) / 4) * (wallClimbCheckLen * 1.9f))), -transform.up, out hit, 10f, groundLayer);
                Debug.DrawRay(playerCamera.position + ((tempDir + playerCamera.TransformDirection(transform.up) / 4) * (wallClimbCheckLen * 1.9f)), -transform.up * 10f, Color.magenta, 2f);

                RaycastHit underHit;

                Physics.Raycast(legsTransform.position, -Vector3.up, out underHit, 10f, groundLayer);
                Debug.DrawRay(legsTransform.position, -Vector3.up * 10f, Color.yellow, 2f);

                if (!isWallClimbHitFound || underHit.collider == hit.collider) {
                    continue;
                }

                if (underHit.collider == null) print("underMissing");
                if (hit.collider == null) print("cameraCheckMissing");
                if ((underHit.collider != null) && (hit.collider != null)) print("underhit: " + underHit.collider.name + " cameraCheckHit: " + hit.collider.name);

                if (hit.point == Vector3.zero) return;

                isClimbing = true;

                wallClimbFinalPosition = hit.point + (transform.up * 1f);
                wallClimbStartPosition = torsoTransform.position;
                wallClimbTimeStart = Time.time;

                playerAudio.ClimbSound();

                return;
            }
        }
    }

    private void CheckStep()
    {
        foreach (Vector3 dir in wallStepDirections) 
        {
            tempDir = legsTransform.TransformDirection(dir);
            isWallStepLow = Physics.Raycast(legsTransform.position, tempDir, wallClimbCheckLen, stepLayer);
            tempDir = torsoTransform.TransformDirection(dir);
            isWallStepMid = Physics.Raycast(torsoTransform.position, tempDir, wallClimbCheckLen * 2, stepLayer);

            if (isWallStepLow && !isWallStepMid)
            {
                isStepping = true;

                RaycastHit hit;

                Physics.Raycast(((torsoTransform.position) + (tempDir * (wallClimbCheckLen * 2))) + Vector3.up, -transform.up, out hit, 5f, groundLayer);

                wallClimbFinalPosition = hit.point + (transform.up * 1f);
                wallClimbStartPosition = torsoTransform.position;
                wallClimbTimeStart = Time.time;

                return;
            }
        }
    }

    private void CheckMoving()
    {
        isMoving = rb.velocity != Vector3.zero && isGrounded;
    }


    private void HandleClimb(float duration)
    {
        float elapsedTime = Time.time - wallClimbTimeStart;
        float t = elapsedTime / duration;

        transform.position = Vector3.Lerp(wallClimbStartPosition, wallClimbFinalPosition, t);

        if (elapsedTime >= duration)
        {
            transform.position = wallClimbFinalPosition;
            isClimbing = false;
            isStepping = false;
        }
    }

    private void HandleCooldowns() {
        if (dashCooldownReffilOffsetTimer > 0f) {
            dashCooldownReffilOffsetTimer -= Time.deltaTime * 1f;
            return;
        }

        dashCooldownReffilOffsetTimer = Mathf.Clamp(dashCooldownReffilOffsetTimer, 0f, dashCooldownReffilOffsetTimerMax * dashCharges);
        dashCooldownTimer += Time.deltaTime * dashRefillScale;
        dashCooldownTimer = Mathf.Clamp(dashCooldownTimer, 0f, dashCooldownTimerMax);

        if (dashCooldownTimer >= ((dashCooldownTimerMax / dashCharges)) && !dashRefillSound2) {
            playerAudio.DashReffilSound();
            dashRefillSound2 = true;
        }
        if (dashCooldownTimer == (dashCooldownTimerMax) && !dashRefillSound1) {
            playerAudio.DashReffilSound();
            dashRefillSound1 = true;
        }
    }

    private void HandleUI() {
        canJumpUI.enabled = isGrounded ? true : false;  
        canDoubleJumpUI.enabled = canDoubleJump ? true : false;
        
        dashCountUI.value = dashCooldownTimer;
    }

    void OnDrawGizmosSelected()
    {
        if (drawGroundCheck) 
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(legsTransform.position, groundCheckSphereRadius);
        }

        if (drawWallStuckCheck)
        {
            Debug.DrawRay(torsoTransform.position, transform.forward * wallStuckCheckLen, isStuckForward ? Color.red : Color.green);
            Debug.DrawRay(torsoTransform.position, -transform.forward * wallStuckCheckLen, isStuckBack ? Color.red : Color.green);
            Debug.DrawRay(torsoTransform.position, transform.right * wallStuckCheckLen, isStuckRight ? Color.red : Color.green);
            Debug.DrawRay(torsoTransform.position, -transform.right * wallStuckCheckLen, isStuckLeft ? Color.red : Color.green);

            Debug.DrawRay(torsoTransform.position, (transform.forward + transform.right).normalized * wallStuckCheckLen, isStuckForwardR ? Color.red : Color.green);
            Debug.DrawRay(torsoTransform.position, (transform.forward - transform.right).normalized * wallStuckCheckLen, isStuckForwardL ? Color.red : Color.green);

            Debug.DrawRay(torsoTransform.position, (-transform.forward + transform.right).normalized * wallStuckCheckLen, isStuckBackR ? Color.red : Color.green);
            Debug.DrawRay(torsoTransform.position, (-transform.forward - transform.right).normalized * wallStuckCheckLen, isStuckBackL ? Color.red : Color.green);
        }
         
        if (drawWallClimbCheck)
        {
            foreach (Vector3 dir in wallClimbDirections)
            {
                debugDrawDir = legsTransform.TransformDirection(dir);
                Debug.DrawRay(legsTransform.position, debugDrawDir * wallClimbCheckLen, isWallClimbForwardLow ? Color.blue : Color.yellow);
                debugDrawDir = playerCamera.TransformDirection(dir);
                Debug.DrawRay(playerCamera.position, (debugDrawDir + playerCamera.TransformDirection(transform.up) / 4) * wallClimbCheckLen * 2, isWallClimbForwardCamera ? Color.blue : Color.yellow);
            }
        }

        if (drawWallStepCheck)
        {
            foreach (Vector3 dir in wallStepDirections)
            {
                debugDrawDir = legsTransform.TransformDirection(dir);
                Debug.DrawRay(legsTransform.position, debugDrawDir * wallClimbCheckLen, isWallStepLow ? Color.blue : Color.yellow);
                debugDrawDir = torsoTransform.TransformDirection(dir);
                Debug.DrawRay(torsoTransform.position, debugDrawDir * wallClimbCheckLen * 2, isWallStepMid ? Color.blue : Color.yellow);
            }
        }

        //Debug.DrawRay(playerCamera.position, playerCamera.forward * 3, Color.magenta, 0.1f);

    }
}