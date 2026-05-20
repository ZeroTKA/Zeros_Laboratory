using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] WeaponData weaponData; // Scriptable object for your weapon's data
    [SerializeField] AmmoHandler ammoHandler;
    [SerializeField] Transform gunNozzle;

    /// <summary>
    /// Fires an event for a gun shot.
    /// Wire up: AmmoHandler.AShotWasFired
    /// </summary>
    public UnityEvent OnGunShot;

    readonly private List<WeaponData.FireModes> fireModeList = new();

    private WeaponData.FireModes currentFireMode;
    private WeaponData.ShotTypes currentShotType;
    private GameObject weaponProjectilePrefab;
    private GameObject weaponProjectile;
    private InputAction shootAction;
    private float timeWhenWeCanShoot = 0f;
    private bool isBursting = false;
    private bool isReloading = false;
    private bool isTriggerPressed = false;
    private bool isTriggerJustPressed = false;

    // -- Specialty Methods -- //
    void Awake()
    {
        StartErrorChecking();
    }
    private void OnDisable()
    {
        shootAction?.Disable();
        shootAction.performed -= OnShootPerformed; // unsubscribing to Input event for when the shoot button is pressed.
        shootAction.canceled -= OnShootCanceled; // unsubscribing to the Input event for when the shoot button is released.
        StopAllCoroutines();
        isBursting = false;
        isReloading = false;
        isTriggerPressed = false;
        isTriggerJustPressed = false;
    }
    private void OnEnable()
    {
        shootAction?.Enable();
        shootAction.performed += OnShootPerformed; // Subscribing to the Input event for when the shoot button is pressed.
        shootAction.canceled += OnShootCanceled; // Subscribing to the Input event for when the shoot button is released.

    }

    private void Start()
    {
        CacheFireModes();
        weaponProjectilePrefab = weaponData.ProjectilePrefab;
        currentShotType = weaponData.ShotType;
    }

    private void Update()
    {        
        HandleShooting();
    }

    // -- Main Methods -- //
    private void HandleShooting()
    {
        // Trying to short-circuit HandleShooting as much as possible.
        if (!isTriggerPressed) { return; }
        if (isReloading) { return; }
        if (isBursting) { return; }
        if (Time.time < timeWhenWeCanShoot) { return; }
        if (ammoHandler.ClipAmmo == 0) { return; }

        switch (currentFireMode)
        {
            case WeaponData.FireModes.Semi: ShootingSemi(); break;
            case WeaponData.FireModes.Burst:ShootingBurst(); return; // Returning here prevents HandleShooting() from writing to timeWhenWeCanShoot prematurely on the next line.
            case WeaponData.FireModes.Auto: ShootingAuto(); break;
        }
        timeWhenWeCanShoot = Time.time + (1f / weaponData.FireRate);
    }

    /// <summary>
    /// Performs a single-shot firing action if the shoot input was pressed during the current frame.
    /// </summary>
    /// <remarks>This is a Raycasting method. Also note, if the player clicks faster than the fire rate, that click is disregarded completley. 
    /// This is intended to prevent semi-auto weapons being used as autos.</remarks>
    private void ShootingSemi()
    {
        if (isTriggerJustPressed)
        {
            isTriggerJustPressed = false;
            PickShotAndShoot();
        }
    }
    private void ShootingBurst()
    {
        if (isTriggerJustPressed)
        {
            isTriggerJustPressed = false;
            isBursting = true;
            StartCoroutine(BurstShot());
        }
    }
    private void ShootingAuto()
    {
        if (isTriggerPressed) // held, not just pressed
        {
            PickShotAndShoot();
        }
    }

    // -- Supplemental Methods -- //
    private void CacheFireModes()
    {
        fireModeList.Clear();
        foreach (WeaponData.FireModes mode in System.Enum.GetValues(typeof(WeaponData.FireModes)))
        {
            if (weaponData.AvailableFireModes.HasFlag(mode))
                fireModeList.Add(mode);
        }
        currentFireMode = fireModeList[0];
    }
    private void PickShotAndShoot()
    {
        switch (currentShotType)
        {
            case WeaponData.ShotTypes.Projectile: ProjectileShot(); break;
            case WeaponData.ShotTypes.Raycast: RaycastShot(); break;
        }
        OnGunShot?.Invoke();
    }
    private void ProjectileShot()
    {
        weaponProjectile = PoolManager.Instance.Rent(weaponProjectilePrefab);
        weaponProjectile.transform.SetPositionAndRotation(gunNozzle.position, gunNozzle.rotation);
    }
    private void RaycastShot()
    {
        Ray ray = new(mainCamera.transform.position, mainCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, weaponData.EffectiveRange))
        {
            Debug.Log(hit.collider.gameObject.name);
            // do something with what you hit.
        }
    }
    /// <summary>
    /// Initializes error checking for the script.
    /// </summary>
    /// <remarks>We try to catch nulls when we start.</remarks>
    private void StartErrorChecking()
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
        if (ammoHandler == null)
        {
            Debug.LogError("[Shooting] Ammo Handler is null.");
        }
    }
    private void OnShootPerformed(InputAction.CallbackContext _) { isTriggerPressed = true; isTriggerJustPressed = true; }
    private void OnShootCanceled(InputAction.CallbackContext _) { isTriggerPressed = false; }

    // -- Coroutines -- //
    /// <summary>
    /// Burst shot handles timeWhenWeCanShoot on its own. Otherwise HandleShooting() would prematurely write to it.
    /// </summary>
    private IEnumerator BurstShot()
    {
        float burstDelay = weaponData.BurstDelay;
        for (int i = 0; i < weaponData.BurstCount; i++)
        {
            if(ammoHandler.ClipAmmo == 0) { break; }
            PickShotAndShoot();
            yield return new WaitForSeconds(burstDelay);
        }
        timeWhenWeCanShoot = Time.time + (1f / weaponData.FireRate);
        isBursting = false;
    }
    public void SetIsReloadingTrue()
    {
        isReloading = true;
    }    
    public void SetIsReloadingFalse()
    {
        isReloading = false;
    }
    public void WeaponSwapped(WeaponData swappedWeapon)
    {
        weaponData = swappedWeapon;
        weaponProjectilePrefab = weaponData.ProjectilePrefab;
        currentShotType = weaponData.ShotType;
        CacheFireModes();
    }
}
