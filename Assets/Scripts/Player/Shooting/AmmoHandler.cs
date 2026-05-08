using System;
using System.Collections;
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
    [SerializeField] WeaponData weaponData;
    private int _clipAmmo;
    private int maxClipAmmo;
    private int reserveAmmo;
    private int maxReserveAmmo;

    private bool isReloading = false;

    public int ClipAmmo => _clipAmmo;

    private InputAction reloadAction;

    // -- Specialty Methods -- //
    void Awake()
    {
        StartErrorChecking();
    }
    private void Update()
    {
        if(reloadAction.WasPressedThisFrame() && !isReloading)
        {
            if (_clipAmmo == maxClipAmmo) { return; }
            if (reserveAmmo == 0) { return; }
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
        if(maxClipAmmo - _clipAmmo <= reserveAmmo)
        {
            reserveAmmo -= maxClipAmmo - _clipAmmo;
            _clipAmmo = maxClipAmmo;            
        }
        else if(maxClipAmmo - _clipAmmo > reserveAmmo)
        {
            _clipAmmo += reserveAmmo;
            reserveAmmo = 0;
        }
        OnReloadingFinished?.Invoke();
        isReloading = false;
    }
}
