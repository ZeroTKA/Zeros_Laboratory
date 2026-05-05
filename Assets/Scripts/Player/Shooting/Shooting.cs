using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] WeaponData weaponData; // Scriptable object for your weapon's data
    [SerializeField] AmmoHandler ammoHandler;
    [SerializeField] Transform gunNozzle;

    /// <summary>
    /// Fired every time a shot is taken.
    /// Default subscriptions: 
    /// AmmoHandler // I'm aware we have a reference already in script. The idea is to have AmmoHandler.AShotWasFired()
    ///                be private to avoid accidentially being called and causing a rukus.
    /// To subscribe: Shooting.OnShoot += YourMethod; // this goes in your script
    /// To unsubscribe: Shooting.OnShoot -= YourMethod; // this goes in your script
    /// </summary>
    public event Action OnShoot;

    // -- Input Actions -- //  -- To add an action, make sure to add in OnDisable, OnEnable, and in StartErrorChecking.
    private InputAction shootAction;
    private InputAction reloadAction;

    private WeaponData.FireModes currentFireMode;
    private WeaponData.ShotTypes currentShotType;
    private GameObject weaponProjectilePrefab;
    private GameObject weaponProjectile;
    private float timeWhenWeCanShoot = 0f;
    private bool isBursting = false;
    private bool isReloading = false;

    private List<WeaponData.FireModes> fireModeList = new();

    // -- Specialty Methods -- //
    void Awake()
    {
        StartErrorChecking();
    }

    private void OnEnable()
    {
        shootAction?.Enable();
        reloadAction?.Enable();
    }

    private void OnDisable()
    {
        shootAction?.Disable();
        reloadAction?.Disable();
    }

    private void Start()
    {
        CacheFireModes();
        weaponProjectilePrefab = weaponData.ProjectilePrefab;
    }

    private void Update()
    {
        HandleShooting();
        HandleReloading();
    }

    // -- Main Methods -- //
    private void HandleShooting()
    {
        if (ammoHandler.ClipAmmo == 0) { return; }
        if (Time.time < timeWhenWeCanShoot) { return; }
        if (isReloading) { return; }

        switch (currentFireMode)
        {
            case WeaponData.FireModes.Semi: ShootingSemi(); break;
            case WeaponData.FireModes.Burst:
                if (!isBursting)
                {
                    ShootingBurst();
                    return; // Returning here prevents HandleShooting() from writing to timeWhenWeCanShoot prematurely.
                }
                break;
            case WeaponData.FireModes.Auto: ShootingAuto(); break;
        }
        timeWhenWeCanShoot = Time.time + (1f / weaponData.FireRate);
    }
    private void HandleReloading()
    {
        if (reloadAction.WasPressedThisFrame() && !isReloading && !isBursting && ammoHandler.CanWeReload())
        {
            isReloading = true;
            StartCoroutine(ReloadTimer()); //after we wait a duration we set isReloading = false.
        }
    }
    /// <summary>
    /// Performs a single-shot firing action if the shoot input was pressed during the current frame.
    /// </summary>
    /// <remarks>This is a Raycasting method. Also note, if the player clicks faster than the fire rate, that click is disregarded completley. 
    /// This is intended to prevent semi-auto weapons being used as autos.</remarks>
    private void ShootingSemi()
    {
        if (shootAction.WasPressedThisFrame())
        {
            PickShotAndShoot();
        }
    }
    private void ShootingBurst()
    {
        if (shootAction.WasPressedThisFrame())
        {
            isBursting = true;
            StartCoroutine(BurstShot());
        }
    }
    private void ShootingAuto()
    {
        if (shootAction.IsPressed())
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
        OnShoot?.Invoke();
    }
    private void ProjectileShot()
    {
        weaponProjectile = PoolManager.Instance.Rent(weaponProjectilePrefab);
        weaponProjectile.transform.SetPositionAndRotation(gunNozzle.transform.position, gunNozzle.transform.rotation);
    }
    private void RaycastShot()
    {
        Ray ray = new(mainCamera.transform.position, mainCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, weaponData.Range))
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
            reloadAction = playerInput.actions["Reload"];
            if (shootAction == null) Debug.LogError("[Shooting] shootAction not found.");
            if (reloadAction == null) Debug.LogError("[Shooting] reloadAction not found.");
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
    private IEnumerator ReloadTimer()
    {
        yield return new WaitForSeconds(weaponData.ReloadTime);
        isReloading = false;
    }
}
