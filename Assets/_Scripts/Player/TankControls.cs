using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class TankControls : MonoBehaviour
{
    private CharacterController controller;
    private MovementState movementState;
    private AnimationDriver animationDriver;
    
    [SerializeField] private Gun gun;
    
    [Header("Movement Settings")]
    public float speed = 10f;
    public float turnSpeed = 100f;
    public float sprintMultiplier = 1.5f;
    public float backwardMultiplier = 0.6f;

    [Header("Quick Turn Settings")]
    public float quickTurnTime = 0.25f;
    public float quickTurnCooldown = 0.5f;

    public bool IsQuickTurning { get; private set; }
    public bool IsQuickTurnOnCooldown => quickTurnCooldownTimer > 0f;

    public float VerticalInput { get; private set; }
    public float HorizontalInput { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool IsAiming { get; private set; }
    public float AimVerticalInput { get; private set; }
    public float AimAngle { get; private set; } = 0f; 
    public float aimAngleSpeed = 50f;

    private float quickTurnCooldownTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        movementState = GetComponent<MovementState>();
        animationDriver = GetComponent<AnimationDriver>();
        }

    void Update()
    {
        HandleInput();
        HandleQuickTurn();
        HandleMovement();
        HandleAiming();
    }

    private void HandleInput()
    {
        VerticalInput = Input.GetAxis("Vertical");
        HorizontalInput = Input.GetAxis("Horizontal");
        IsSprinting = Input.GetKey(KeyCode.LeftShift) && VerticalInput > 0.1f;

        if (Input.GetKeyDown(KeyCode.Space))
            IsAiming = !IsAiming;

        AimVerticalInput = IsAiming ? Input.GetAxis("Vertical") : 0f;
    }

    private void HandleQuickTurn()
    {
        // Reduce cooldown timer
        if (quickTurnCooldownTimer > 0f)
            quickTurnCooldownTimer -= Time.deltaTime;

        // Quick Turn input — only if allowed
        if (!IsQuickTurning && Input.GetKey(KeyCode.Q) && quickTurnCooldownTimer <= 0f)
        {
            StartCoroutine(QuickTurnCoroutine());
        }
    }

    private void HandleMovement()
    {
        if (IsAiming)
        {
            // Aiming mode: restrict movement to turning and aiming
            transform.Rotate(0, HorizontalInput * turnSpeed * Time.deltaTime, 0);
        }
        else if (movementState.CanMove)
        {
            // Handle rotation
            transform.Rotate(0, HorizontalInput * turnSpeed * Time.deltaTime, 0);

            // Determine movement speed
            float currentSpeed = CalculateMovementSpeed();

            // Apply movement
            Vector3 moveDir = transform.forward * VerticalInput * currentSpeed;
            controller.Move(moveDir * Time.deltaTime - Vector3.up * 0.1f); // Gravity adjustment
        }
    }

    private void HandleAiming()
    {
        if (IsAiming)
        {
            if (Input.GetKey(KeyCode.W))
            {
                AimAngle += aimAngleSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                AimAngle -= aimAngleSpeed * Time.deltaTime;
            }
            // AimAngle debug message
            

            AimAngle = Mathf.Clamp(AimAngle, -45f, 45f); 

            if (Input.GetKeyDown(KeyCode.J))
            {
                gun.TryShoot();
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                gun.TryReload();
                
            }
             
        }
    }

    private float CalculateMovementSpeed()
    {
        if (VerticalInput > 0.1f)
            return IsSprinting ? speed * sprintMultiplier : speed;
        else if (VerticalInput < -0.1f)
            return speed * backwardMultiplier;

        return 0f;
    }

    private IEnumerator QuickTurnCoroutine()
    {
        IsQuickTurning = true;

        Quaternion startRot = transform.rotation;
        Quaternion endRot = transform.rotation * Quaternion.Euler(0f, 180f, 0f);

        float elapsed = 0f;

        while (elapsed < quickTurnTime)
        {
            float t = elapsed / quickTurnTime;
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRot;

        // Start cooldown
        quickTurnCooldownTimer = quickTurnCooldown;
        IsQuickTurning = false;
    }
}
