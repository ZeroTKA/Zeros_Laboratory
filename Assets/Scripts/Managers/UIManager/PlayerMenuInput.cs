using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class PlayerMenuInput : MonoBehaviour
{
    // -- Unite Events -- //
    public UnityEvent OnEscActionPress; // wire up to UI Manager.EscapeMenuButtonPressed


    // -- Input Actions -- //  Make sure to add in Project Settings, OnDisable, OnEnable, and in StartErrorChecking.
    private InputAction escAction;

    // -- Specialty Methods -- //
    private void Awake()
    {
        StartErrorChecking();
    }
    private void Update()
    {
        if(escAction.WasPressedThisFrame())
        {
            OnEscActionPress?.Invoke();
        }
    }
    private void OnDisable()
    {
        escAction?.Disable();
    }
    private void OnEnable()
    {
        escAction?.Enable();
    }

    // -- Supplemental Methods -- //
    private void StartErrorChecking()
    {
        if (TryGetComponent<PlayerInput>(out var playerInput))
        {
            escAction = playerInput.actions["Escape"];
            if (escAction == null) Debug.LogError("[PlayerMenuInput] Escape action not found.");
        }
        else
        {
            Debug.LogError("[PlayerMenuInput] Unable to find PlayerInput.");
        }
    }
}
