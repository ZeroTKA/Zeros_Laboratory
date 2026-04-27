using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    // -- Input Actions -- //  -- To add an action, make sure to add in OnDisable, OnEnable, and in StartErrorChecking.
    private InputAction shootAction;

    [SerializeField] Camera mainCamera;
    [SerializeField] WeaponData weaponData; // Scriptable object for your weapon's data
    [SerializeField] AmmoHandler ammoHandler;

    WeaponData.FireModes currentFireMode = WeaponData.FireModes.Semi;

    // -- Specialty Methods -- //
    void Awake()
    {
        StartErrorChecking();
    }

    private void OnEnable()
    {
        shootAction?.Enable();
    }

    private void OnDisable()
    {
        shootAction?.Disable();
    }


    // -- Main Methods -- //
    void HandleShooting()
    {
        if (ammoHandler.AmmoInClip == 0) { return; }

        switch (currentFireMode)
        {
            case WeaponData.FireModes.Semi:
                ShootingSemi();
                break;
            case WeaponData.FireModes.Burst:
                ShootingBurst();
                break;
            case WeaponData.FireModes.Auto:
                ShootingAuto();
                break;
        }
    }
    void Reload()
    {

    }
    void ShootingSemi()
    {
        if (shootAction.WasPressedThisFrame())
        {
            Ray ray = new(mainCamera.transform.position, mainCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, weaponData.Range))
            {
                Debug.Log(hit.collider.gameObject.name);
            }
        }
    }
    void ShootingBurst()
    {
        if (shootAction.IsPressed())
        {
            // do burst things with coroutines?
        }
    }
    void ShootingAuto()
    {
        if (shootAction.IsPressed())
        {
            Ray ray = new(mainCamera.transform.position, mainCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, weaponData.Range))
            {
                Debug.Log(hit.collider.gameObject.name);
            }
        }
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
            Debug.LogError("[Shooting] Unable to find PlayerInput attached to this object.");
        }
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            Debug.LogWarning("[Shooting] Main camera not set, setting to camera.main");
        }
        if (weaponData == null)
        {
            Debug.LogError("[Shooting] Weapon Data is null.");
        }
    }
}
