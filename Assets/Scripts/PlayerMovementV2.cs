using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovementV2 : MonoBehaviour
{
    private enum MovementState { Grounded, Airborne }
    private enum Stance { Standing, Crouching, Proning }
    private MovementState currentState;
    private Stance currentStance;

    //-- Input Actions --//  -- To add an action, make sure to add in OnDisable, OnEnable, and in StartErrorChecking.
    private InputAction lookAction;
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    private InputAction proneAction;

    [Header("Character Setup")]
    [SerializeField] private CharacterController controller; // Drag your character controller here
    [SerializeField] private Camera playerCamera; // Drag your player camera here
    [SerializeField] private Transform cameraPivotTransform; // Drag your camera pivot here
    private Vector3 standingColliderCenter;
    private float standingColliderHeight;
    [SerializeField] float standHeight = .4f; // camera's perspective
    [SerializeField] float crouchHeight = .29f; // camera's perspective
    [SerializeField] float proneHeight = .1f; // camera's perspective

    [Header("Movement Variables")]
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float runSpeed = 9f;
    [SerializeField] float crouchSpeed = 4f;
    [SerializeField] float proneSpeed = 2f;
    float baseSpeed;

    [Tooltip("This is movement speed while aireborn.")]
    [SerializeField] float airSpeed = 5f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpHeight = 1f;
    private Vector3 velocity; // used for gravity. Velocity.y persistant.
    // -- Specialty Methods -- //

    private void Awake()
    {
        StartErrorChecking();
        standingColliderHeight = controller.height; // grabbing the initial height for the controller at a standing position. Assuming it starts standing.
        standingColliderCenter = controller.center;
        baseSpeed = walkSpeed;
    }
    void Update()
    {
        ApplyGravity();
        UpdateState();
        HandleState();
    }
    private void OnEnable()
    {
        lookAction?.Enable();
        moveAction?.Enable();
        sprintAction?.Enable();
        jumpAction?.Enable();
        crouchAction?.Enable();
        proneAction?.Enable();
    }
    private void OnDisable()
    {
        lookAction?.Disable();
        moveAction?.Disable();
        sprintAction?.Disable();
        jumpAction?.Disable();
        crouchAction?.Disable();
        proneAction?.Disable();
    }

    // -- Main Methods -- //
    private void HandleAirborne()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 inputMove = (transform.right * input.x + transform.forward * input.y) * airSpeed;
        controller.Move((inputMove + velocity) * Time.deltaTime);
    }
    private void HandleGrounded()
    {
        if (crouchAction.WasPressedThisFrame())
        {
            currentStance = currentStance == Stance.Crouching ? Stance.Standing : Stance.Crouching;
            SetStance(currentStance);
        }
        if (proneAction.WasPressedThisFrame())
        {
            currentStance = currentStance == Stance.Proning ? Stance.Standing : Stance.Proning;
            SetStance(currentStance);
        }
        if (currentStance == Stance.Standing) // only allow jumping and running while standing
        {
            HandleJump();
            baseSpeed = IsRunning() ? runSpeed : walkSpeed;
        }
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 inputMove = (transform.right * input.x + transform.forward * input.y) * baseSpeed;
        controller.Move((inputMove + velocity) * Time.deltaTime);

    }
    private void HandleJump()
    {
        if (jumpAction.WasPressedThisFrame()) // WasPressedThisFrame gets us a single press even if the button is held.
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            currentState = MovementState.Airborne;
        }
    }
    private void HandleState()
    {
        switch (currentState)
        {
            case MovementState.Airborne:
                HandleAirborne();
                break;
            case MovementState.Grounded:
                HandleGrounded();
                break;
        }
    }
    private void UpdateState()
    {
        switch (currentState)
        {
            case MovementState.Grounded:
                if (!controller.isGrounded)
                    currentState = MovementState.Airborne;
                break;
            case MovementState.Airborne:
                if (controller.isGrounded)
                    currentState = MovementState.Grounded;
                break;
        }
    }

    // -- Supplemental Methods -- //
    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
    }
    private bool IsRunning()
    {
        return sprintAction.IsPressed() && moveAction.ReadValue<Vector2>().magnitude > 0.1f;
    }
    private void StartErrorChecking()
    {
        if (controller == null) Debug.LogError("[PlayerMovementV2] CharacterController not assigned.");
        if (playerCamera == null) Debug.LogError("[PlayerMovementV2] playerCamera not assigned.");
        if (cameraPivotTransform == null) Debug.LogError("[PlayerMovementV2] Camera Transform not assigned.");

        if (TryGetComponent<PlayerInput>(out var playerInput))
        {
            moveAction = playerInput.actions["Move"];
            lookAction = playerInput.actions["Look"];
            sprintAction = playerInput.actions["Sprint"];
            jumpAction = playerInput.actions["Jump"];
            crouchAction = playerInput.actions["Crouch"];
            proneAction = playerInput.actions["Prone"];

            if (lookAction == null) Debug.LogError("[PlayerMovementV2] Look action not found.");
            if (moveAction == null) Debug.LogError("[PlayerMovementV2] Move action not found.");
            if (sprintAction == null) Debug.LogError("[PlayerMovementV2] Sprint action not found.");
            if (jumpAction == null) Debug.LogError("[PlayerMovementV2] Jump action not found.");
            if (crouchAction == null) Debug.LogError("[PlayerMovementV2] Crouch action not found.");
            if (proneAction == null) Debug.LogError("[PlayerMovementV2] Prone action not found.");
        }
        else
        {
            Debug.LogError("[PlayerMovementV2] Unable to find PlayerInput.");
        }
    }
    private void SetStance(Stance stance)
    {
        if (controller == null) return;

        switch (stance)
        {
            case Stance.Standing:

                controller.height = standingColliderHeight;
                controller.center = standingColliderCenter;
                break;

            case Stance.Crouching:
                {
                    float cameraDelta = standHeight - crouchHeight;
                    float targetControllerHeight = standingColliderHeight - cameraDelta;
                    Vector3 targetCenter = new(
                        standingColliderCenter.x,
                        standingColliderCenter.y - cameraDelta / 2f,
                        standingColliderCenter.z);
                    controller.height = targetControllerHeight;
                    controller.center = targetCenter;
                    baseSpeed = crouchSpeed;
                }
                break;

            case Stance.Proning:
                {
                    float cameraDelta = standHeight - proneHeight;
                    float targetControllerHeight = standingColliderHeight - cameraDelta;
                    Vector3 targetCenter = new(
                        standingColliderCenter.x,
                        standingColliderCenter.y - cameraDelta / 2f,
                        standingColliderCenter.z);
                    controller.height = targetControllerHeight;
                    controller.center = targetCenter;
                    baseSpeed = proneSpeed;
                }
                break;
        }
    }

}
