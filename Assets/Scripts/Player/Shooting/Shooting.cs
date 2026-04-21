using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{

    private InputAction shootAction;

    void Awake()
    {
        StartErrorChecking();
    }
    

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
