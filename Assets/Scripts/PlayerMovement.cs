using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// -- To Do List -- 
    /// 1. When crouching, setting the controller height isn't lerpd. It's immediate. Do we care?
    /// </summary>
    [SerializeField] private CharacterController controller; // Drag your character controller here

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

    //-- Crouching Variables --//
    private Vector3 standingColliderCenter;
    private float standingColliderHeight;
    private float standHeight = 1f; // camera's perspective
    private float crouchHeight = .65f; // camera's perspective
    private float cameraLerpSpeed = 5f;
    private float targetHeight = 0f;
    readonly private float changeInCrouchSpeed = 3f;
    private bool isCrouched = false;

    //-- Prone Variables --//
    private bool isProne = false;
    private float proneHeight = .3f;

    //-- Input Actions --//  -- To add an action, make sure to add in OnDisable, OnEnable, and in StartErrorChecking.
    private InputAction lookAction;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    private InputAction proneAction;

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
        Crouch();
        Prone();
        ChangeCameraHeight();
        Rotate();
        Movement();
        Jump();
        

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
    }
    private void OnDisable()
    {
        lookAction?.Disable();
        moveAction?.Disable();
        jumpAction?.Disable();
        crouchAction?.Disable();
        proneAction?.Disable();
    }

    // -- Main Methods -- //
    private void Crouch()
    {
        if (crouchAction == null) return;
        if(crouchAction.triggered)
        {
            isCrouched = !isCrouched;
            if (isCrouched) isProne = false;
            SetCharacterControllerHeightAndCenter(isCrouched ? Stance.Crouched : Stance.Standing); // Adjust Player Controller Height 
            if (isCrouched) { currentMoveSpeed -= changeInCrouchSpeed; } else { currentMoveSpeed += changeInCrouchSpeed; } // Adjust move speed            
            currentMoveSpeed = baseMoveSpeed - (isCrouched ? changeInCrouchSpeed : 0f);
        }    
    }
    private void Jump()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y <= 0)
        {
            velocity.y = -2f;
        }
        if (jumpAction == null) return;
        if (jumpAction.triggered && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // if we intend to double jump we should velocity.y +=
        }
        velocity.y += gravity * Time.deltaTime; // Gravity always applies
    }
    private void Movement()
    {
        if (moveAction == null) return;

        Vector2 input = moveAction.ReadValue<Vector2>();
        inputMove = (transform.right * input.x + transform.forward * input.y) * currentMoveSpeed;

    }
    private void Prone()
    {
        if (proneAction == null) return;
        if (proneAction.triggered)
        {
            isProne = !isProne;
            if (isProne) isCrouched = false;
            SetCharacterControllerHeightAndCenter(isProne ? Stance.Prone : Stance.Standing); // Adjust Player Controller Height 

            if (isProne) { currentMoveSpeed -= changeInCrouchSpeed; } else { currentMoveSpeed += changeInCrouchSpeed; } // Adjust move speed            
            currentMoveSpeed = baseMoveSpeed - (isProne ? changeInCrouchSpeed : 0f);
        }
        // -- Prep for lerping each frame. -- //
        Vector3 pos = cameraPivotTransform.localPosition;
        // camera target depends on current stance (prone overrides crouch)
        if (isProne) targetHeight = proneHeight;
        else targetHeight = isCrouched ? crouchHeight : standHeight;

        if (Mathf.Abs(pos.y - targetHeight) > 0.001f)
        {
            // -- Slide Camera down -- //
            pos.y = Mathf.Lerp(pos.y, targetHeight, Time.deltaTime * cameraLerpSpeed);
            cameraPivotTransform.localPosition = pos;
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
                    Vector3 targetCenter = new Vector3(
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
                    Vector3 targetCenter = new Vector3(
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
        }
        else
        {
            Debug.LogError("[PlayerMovement] Unable to find PlayerInput.");
        }
    }
}
