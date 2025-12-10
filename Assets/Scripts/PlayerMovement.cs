using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// -- To Do List -- 
    /// 1. When crouching, setting the controller height isn't lerpd. It's immediate. Do we care?
    /// </summary>

    ///<summary>
    /// to add an action do the following:
    /// OnEnable
    /// OnDisable
    /// StartErrorChecking
    /// Make its own Method
    /// </summary>
    [SerializeField] private CharacterController controller; // Drag your character controller here
    [SerializeField] private Camera playerCamera; // Drag your player camera here

    //-- Rotation Variables --//
    [SerializeField] private Transform cameraPivotTransform; // Drag your camera pivot here
    float xRotation = 0f;
    readonly float mouseSensitivity = 5f;

    //--Gravity --//
    private Vector3 velocity;
    readonly float gravity = -9.81f;
    readonly float jumpHeight = 1f;

    //-- Movement Variables --//
    private float baseMoveSpeed = 5f;
    float currentMoveSpeed;
    Vector3 inputMove;
    bool isGrounded;

    //-- Standing Variables --//
    private Vector3 standingColliderCenter;
    private float standingColliderHeight;

    //-- Crouching Variables --//
    private float standHeight = 1f; // camera's perspective
    private float crouchHeight = .65f; // camera's perspective
    readonly private float crouchSpeed = .6f; // as in move at a rate of 60% of base move speed.
    private bool isCrouched = false;

    //-- Prone Variables --//
    private bool isProne = false;
    private float proneHeight = .3f;
    private readonly float proneSpeed = .3f; // as in move at a rate of 30% of base move speed.

    // -- Interaction Variables --//
    private float interactDistance = 3f;

    //-- Input Actions --//  -- To add an action, make sure to add in OnDisable, OnEnable, and in StartErrorChecking.
    private InputAction lookAction;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    private InputAction proneAction;
    private InputAction interactAction;

    private float cameraLerpSpeed = 5f;

    // Stance enum to make intent explicit
    private enum Stance { Standing, Crouched, Prone }

    // -- Specialty Methods -- //
    private void Start()
    {
        StartErrorChecking();
        Cursor.lockState = CursorLockMode.Locked;
        standingColliderHeight = controller.height;
        // grabbing the initial height for the controller at a standing position. Assuming it starts standing.
        standingColliderCenter = controller.center;
        currentMoveSpeed = baseMoveSpeed;
    }

    private void Update()
    {
        // -- Player Button Pushes -- //
        Movement(); // movement should be before jump, crouch, and prone to get isGrounded. It might break otherwise.
        Rotate();
        Crouch();
        Prone();
        ChangeCameraHeight();
        Jump();
        Interact();
        if (controller == null)
        {
            Debug.LogError("[PlayerMovment] Can't find the controller in Update");
        }
        else
        {
            controller.Move((inputMove + velocity) * Time.deltaTime);
        }
    }
    private void OnEnable()
    {
        lookAction?.Enable();
        moveAction?.Enable();
        jumpAction?.Enable();
        crouchAction?.Enable();
        proneAction?.Enable();
        interactAction?.Enable();
    }
    private void OnDisable()
    {
        lookAction?.Disable();
        moveAction?.Disable();
        jumpAction?.Disable();
        crouchAction?.Disable();
        proneAction?.Disable();
        interactAction?.Disable();
    }

    // -- Main Methods -- //
    private void Crouch()
    {
        if (crouchAction == null) return;
        if (crouchAction.triggered && isGrounded)
        {
            isCrouched = !isCrouched;
            if (isCrouched) isProne = false; // you can't be both prone and crouched.
            SetCharacterControllerHeightAndCenter(isCrouched ? Stance.Crouched : Stance.Standing); // Adjust Player Controller Height           
            currentMoveSpeed = isCrouched ? baseMoveSpeed * crouchSpeed : baseMoveSpeed; // Adjust move speed
        }
    }
    private void Interact()
    {
        if (interactAction == null) return;
        if (interactAction.triggered)
        {
            Debug.Log("Left");
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                if (hit.transform.gameObject.TryGetComponent<Buttons>(out Buttons component))
                {
                    component.ButtonPressed();
                }
            }
        }
    }
    private void Jump()
    {
        if (jumpAction == null) return;
        if (jumpAction.triggered && isGrounded && !isCrouched && !isProne)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // if we intend to double jump we should velocity.y +=
        }
        velocity.y += gravity * Time.deltaTime; // Gravity always applies

        // I could just program if jump action trigger and I'm crouched, to stand and if I'm prone to stand? I don't know.
    }
    private void Movement()
    {
        if (moveAction == null) return;

        Vector2 input = moveAction.ReadValue<Vector2>();
        inputMove = (transform.right * input.x + transform.forward * input.y) * currentMoveSpeed; // magic of movement.

        // -- Status checks for jump, essentially. -- //
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y <= 0)
        {
            velocity.y = -2f;
        }
    }
    private void Prone()
    {
        if (proneAction == null) return;
        if (proneAction.triggered && isGrounded)
        {
            isProne = !isProne;
            if (isProne) isCrouched = false; // You can't be both prone and crouched.
            SetCharacterControllerHeightAndCenter(isProne ? Stance.Prone : Stance.Standing); // Adjust Player Controller Height          
            currentMoveSpeed = isProne ? baseMoveSpeed * proneSpeed : baseMoveSpeed; // Adjust move speed 
        }
    }
    private void Rotate()
    {
        if (lookAction == null) return;

        Vector2 mouseDelta = mouseSensitivity * Time.deltaTime * lookAction.ReadValue<Vector2>();

        // Horizontal rotation (Y-axis)
        transform.Rotate(Vector3.up * mouseDelta.x);

        // Vertical rotation (X-axis)
        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cameraPivotTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    // -- Supplemental Methods -- //
    private void ChangeCameraHeight()
    {
        if (cameraPivotTransform == null) return;

        //-- Prep Lerping --//
        float targetHeight;
        if (isProne) targetHeight = proneHeight;
        else targetHeight = isCrouched ? crouchHeight : standHeight;


        Vector3 pos = cameraPivotTransform.localPosition;
        if (Mathf.Abs(pos.y - targetHeight) > 0.001f)
        {
            //-- Slide Camera Down --//
            pos.y = Mathf.Lerp(pos.y, targetHeight, Time.deltaTime * cameraLerpSpeed);
            cameraPivotTransform.localPosition = pos;
        }
    }
    private void SetCharacterControllerHeightAndCenter(Stance stance)
    {
        if (controller == null) return;

        switch (stance)
        {
            case Stance.Standing:
                controller.height = standingColliderHeight;
                controller.center = standingColliderCenter;
                break;

            case Stance.Crouched:
                {
                    float cameraDelta = standHeight - crouchHeight;
                    float targetControllerHeight = standingColliderHeight - cameraDelta;
                    Vector3 targetCenter = new(
                        standingColliderCenter.x,
                        standingColliderCenter.y - cameraDelta / 2f,
                        standingColliderCenter.z);
                    controller.height = targetControllerHeight;
                    controller.center = targetCenter;
                }
                break;

            case Stance.Prone:
                {
                    float cameraDelta = standHeight - proneHeight;
                    float targetControllerHeight = standingColliderHeight - cameraDelta;
                    Vector3 targetCenter = new(
                        standingColliderCenter.x,
                        standingColliderCenter.y - cameraDelta / 2f,
                        standingColliderCenter.z);
                    controller.height = targetControllerHeight;
                    controller.center = targetCenter;
                }
                break;
        }
    }
    private void StartErrorChecking()
    {
        if (controller == null)
            Debug.LogError("[PlayerMovement] CharacterController not assigned.");
        if (cameraPivotTransform == null)
            Debug.LogError("[PlayerMovement] Camera Transform not assigned.");

        if (TryGetComponent<PlayerInput>(out var playerInput))
        {
            lookAction = playerInput.actions["Look"];
            moveAction = playerInput.actions["Move"];
            jumpAction = playerInput.actions["Jump"];
            crouchAction = playerInput.actions["Crouch"];
            proneAction = playerInput.actions["Prone"];
            interactAction = playerInput.actions["Interact"];
        }
        else
        {
            Debug.LogError("[PlayerMovement] Unable to find PlayerInput.");
        }
    }
}
