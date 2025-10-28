using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
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
    readonly float moveSpeed = 5f;
    Vector3 inputMove;
    bool isGrounded;

    //-- Crouching Variables --//
    private float standHeight = 1f;
    private float crouchHeight = .65f;
    private float crouchSpeed = 5f;
    private float targetHeight = 0f;
    private bool isCrouched = false;

    //-- Input Actions --//  -- To add an action, make sure to add in OnDisable, OnEnable, and in StartErrorChecking.
    private InputAction lookAction;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction crouchAction;

    // -- Specialty Methods -- //
    private void Start()
    {
        StartErrorChecking();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // -- Player Button Pushes -- //
        Crouch();
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
    }
    private void OnDisable()
    {
        lookAction?.Disable();
        moveAction?.Disable();
        jumpAction?.Disable();
        crouchAction?.Disable();
    }

    // -- Main Methods -- //
    private void Crouch()
    {
        if (crouchAction == null) return;
        if(crouchAction.triggered)
        {
            isCrouched = !isCrouched;
        }
        // -- Prep for crouching -- //
        Vector3 pos = cameraPivotTransform.localPosition;
        targetHeight = isCrouched ? crouchHeight : standHeight;

        if (Mathf.Abs(pos.y - targetHeight) > 0.001f)
        {
            // -- Slide Camera down -- //
            pos.y = Mathf.Lerp(pos.y, targetHeight, Time.deltaTime * crouchSpeed);
            cameraPivotTransform.localPosition = pos;
        }
        
        // -- Adjust Player Controller Height -- //


        // -- adjust move speed -- //



        if (crouchAction == null) return;


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
        inputMove = (transform.right * input.x + transform.forward * input.y) * moveSpeed;

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
        }
        else
        {
            Debug.LogError("[PlayerMovement] Unable to find PlayerInput.");
        }
    }
}
