using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    // -- Input Actions -- //  -- To add an action, make sure to add in OnDisable, OnEnable, and in StartErrorChecking.
    private InputAction shootAction;

    [SerializeField] Camera mainCamera;
    [SerializeField] WeaponData weaponData; // Scriptable object for your weapon's data
    [SerializeField] AmmoHandler AmmoHandler;

    WeaponData.FireModes currentFireMode = WeaponData.FireModes.Semi;
    float timeWhenWeCanShoot = 0f;

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
        if (AmmoHandler.AmmoInClip == 0) { return; }
        if (Time.time < timeWhenWeCanShoot) { return; }

        switch (currentFireMode)
        {
            case WeaponData.FireModes.Semi:
                ShootingSemi(); // see summary about dropping clicks that are too fast
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
    /// <summary>
    /// Performs a single-shot firing action if the shoot input was pressed during the current frame.
    /// </summary>
    /// <remarks>This is a Raycasting method. Also note, if the player clicks faster than the fire rate, that click is disregarded completley. 
    /// This is intended to prevent semi-auto weapons being used as autos.</remarks>
    void ShootingSemi()
    {
        if (shootAction.WasPressedThisFrame())
        {
            Ray ray = new(mainCamera.transform.position, mainCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, weaponData.Range))
            {
                Debug.Log(hit.collider.gameObject.name);
                // do something with what you hit.
            }
            timeWhenWeCanShoot = Time.time + (1f / weaponData.FireRate);
            AmmoHandler.AShotWasFired();
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
