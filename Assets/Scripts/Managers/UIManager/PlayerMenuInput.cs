using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMenuInput : MonoBehaviour
{
    //-- Input Actions --//  -- To add an action, make sure to add in OnDisable, OnEnable, and in StartErrorChecking.
    private InputAction escAction;

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
