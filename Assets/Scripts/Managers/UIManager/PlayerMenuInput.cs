using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMenuInput : MonoBehaviour
{
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
            if(UIManager.Instance.CurrentState != UIManager.UIState.Pause)
            {
                UIManager.Instance.ChangeState(UIManager.UIState.Pause);
            }
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
