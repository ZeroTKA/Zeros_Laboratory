using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class AmmoHandler : MonoBehaviour
{
    /// <summary>
    /// Fires when reloading begins.
    /// Wire up: Shooting.SetIsReloadingTrue
    /// </summary>
    public UnityEvent OnReloading;

    /// <summary>
    /// Fires when reloading is complete.
    /// Wire up: Shooting.SetIsReloadingFalse
    /// </summary>
    public UnityEvent OnReloadingFinished;
    private struct AmmoState
    {
        public int ammoInClip;
        public int reserveAmmo;
    }

    [SerializeField] WeaponData weaponData;
    readonly private Dictionary<WeaponData, AmmoState> ammoDict = new();
    private int _clipAmmo;
    private int _maxClipAmmo;
    private int _reserveAmmo;
    private int _maxReserveAmmo;

    private bool isReloading = false;

    public int ClipAmmo => _clipAmmo;

    private InputAction reloadAction;

    // -- Specialty Methods -- //
    void Awake()
    {
        StartErrorChecking();
    }
    private void Start()
    {
        RegisterFirstWeapon();
    }
    private void Update()
    {
        if (reloadAction.WasPressedThisFrame() && !isReloading)
        {
            if (_clipAmmo == _maxClipAmmo) { return; }
            if (_reserveAmmo == 0) { return; }
            StartCoroutine(Reload());
        }
    }

    // -- Main Methods -- //
    public void AShotWasFired()
    {
        _clipAmmo -= 1;
        if (_clipAmmo < 0)
        {
            Debug.LogWarning("[AmmoHandler] You can't have a negative number in a clip.");
        }
    }

    // -- Supplemental Methods -- //
    private void RegisterFirstWeapon()
    {
        int ammoTemp = weaponData.ClipSize;
        int reserveTemp = weaponData.MaxReserveAmmo;
        _clipAmmo = ammoTemp;
        _maxClipAmmo = ammoTemp;
        _reserveAmmo = reserveTemp - ammoTemp;
        _maxReserveAmmo = reserveTemp;
        ammoDict.Add(weaponData, new AmmoState
        {
            ammoInClip = _clipAmmo,
            reserveAmmo = _reserveAmmo
        });
    }
    private void StartErrorChecking()
    {
        if (TryGetComponent<PlayerInput>(out var playerInput))
        {
            reloadAction = playerInput.actions["Reload"];
            if (reloadAction == null) Debug.LogError("[AmmoHandler] reloadAction not found.");

        }
        else
        {
            Debug.LogError("[AmmoHandler] Unable to find PlayerInput attached to this object.");
        }
    }
    private IEnumerator Reload()
    {
        isReloading = true;
        OnReloading?.Invoke();
        yield return new WaitForSeconds(weaponData.ReloadTime);
        if (_maxClipAmmo - _clipAmmo <= _reserveAmmo)
        {
            _reserveAmmo -= _maxClipAmmo - _clipAmmo;
            _clipAmmo = _maxClipAmmo;
        }
        else if (_maxClipAmmo - _clipAmmo > _reserveAmmo)
        {
            _clipAmmo += _reserveAmmo;
            _reserveAmmo = 0;
        }
        OnReloadingFinished?.Invoke();
        isReloading = false;
    }
    public void WeaponSwapped(WeaponData swappedWeaponData)
    {
        // Save data for the current weapon
        if (ammoDict.TryGetValue(weaponData, out AmmoState currentState))
        {
            currentState.ammoInClip = _clipAmmo;
            currentState.reserveAmmo = _reserveAmmo;
            ammoDict[weaponData] = currentState;
        }

        // Swap
        weaponData = swappedWeaponData;

        // Load data for the swapped weapon.
        if (ammoDict.TryGetValue(weaponData, out AmmoState newState))
        {
            _clipAmmo = newState.ammoInClip;
            _maxClipAmmo = weaponData.ClipSize;
            _reserveAmmo = newState.reserveAmmo;
            _maxReserveAmmo = weaponData.MaxReserveAmmo;
        }
        else // register new weapon
        {
            int ammoTemp = weaponData.ClipSize;
            int reserveTemp = weaponData.MaxReserveAmmo;
            _clipAmmo = ammoTemp;
            _maxClipAmmo = ammoTemp;
            _reserveAmmo = reserveTemp - ammoTemp;
            _maxReserveAmmo = reserveTemp;
            ammoDict.Add(weaponData, new AmmoState
            {
                ammoInClip = _clipAmmo,
                reserveAmmo = _reserveAmmo
            }
            );
        }
    }
}
