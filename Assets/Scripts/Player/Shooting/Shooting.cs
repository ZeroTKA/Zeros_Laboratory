using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{

    private InputAction shootAction;

    void Awake()
    {
        StartErrorChecking();
    }
    // -- Main Methods -- //
    void Shoot()
    { 
    
    }    
    void Reload()
    {

    }

    //-- Supplemental Methods --//
    /// <summary>
    /// Initializes error checking for the script.
    /// </summary>
    /// <remarks>We try to catch nulls when we start.</remarks>
    void StartErrorChecking()
    {
        if (TryGetComponent<PlayerInput>(out var playerInput))
        {
            shootAction = playerInput.actions["Shoot"];
            
            if (shootAction == null) Debug.LogError("[Shooting] shootAction not found.");
        }
        else
        {
            Debug.LogError("[Shootin] Unable to find PlayerInput.");
        }
    }
}
