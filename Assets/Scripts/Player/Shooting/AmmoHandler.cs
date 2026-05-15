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
    public UnityEvent OnReloadingStarting;

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
    readonly private Dictionary<WeaponData, AmmoState> ammoStateDictionary = new();
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
    private void OnDisable()
    {
        reloadAction?.Disable();

        // Hopefully stops the Reload routine and resets isReloading if this gets disabled.
        StopAllCoroutines();
        isReloading = false;
    }
    private void OnEnable()
    {
        reloadAction?.Enable();
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
    /// <summary>
    /// Decreases the ammo count by one. Triggered by OnShoot?.Invoke().
    /// </summary>
    public void AShotWasFired()
    {
        _clipAmmo -= 1;
        if (_clipAmmo < 0)
        {
            Debug.LogWarning("[AmmoHandler] You can't have a negative number in a clip but it happened. Are you calling this multiple times per shot?");
        }
    }
    /// <summary>
    /// Updates the state of ammo for the current weapon and sets corresponding variables for the new weapon.
    /// </summary>    
    /// <param name="swappedWeaponData">The scriptable object to switch to.</param>
    public void WeaponSwapped(WeaponData swappedWeaponData)
    {
        // Error checking.
        if(swappedWeaponData == null)
        {
            Debug.LogError("[AmmoHandler] We can't pass a null WeaponData during a swap. Returning.");
            return;
        }
        if (swappedWeaponData == weaponData)
        {
            Debug.LogWarning("[AmmoHandler] The new WeaponData is already the current WeaponData. Did we pass the wrong one?");
            return;
        } 
            
        // Save data for the current weapon
        if (ammoStateDictionary.TryGetValue(weaponData, out AmmoState currentState))
        {
            currentState.ammoInClip = _clipAmmo;
            currentState.reserveAmmo = _reserveAmmo;
            ammoStateDictionary[weaponData] = currentState;
        }
        else
        {
            Debug.LogError("[AmmoHandler] Hmm. This shouldn't ever happen. " +
                "We should RegisterFirstWeapon() in start() and therefore always always have data registered. What happened??");
        }

        // Swap
        weaponData = swappedWeaponData;

        // Load data for the swapped weapon.
        if (ammoStateDictionary.TryGetValue(weaponData, out AmmoState newState))
        {
            _clipAmmo = newState.ammoInClip;
            _maxClipAmmo = weaponData.ClipSize;
            _reserveAmmo = newState.reserveAmmo;
            _maxReserveAmmo = weaponData.MaxReserveAmmo;
        }
        else // register new weapon
        {
            int ammoTemp = weaponData.ClipSize;
            _clipAmmo = ammoTemp;
            _maxClipAmmo = ammoTemp;
            _reserveAmmo = weaponData.ReserveAmmo;
            _maxReserveAmmo = weaponData.MaxReserveAmmo;

            ammoStateDictionary.Add(weaponData, 
                new AmmoState
                {
                    ammoInClip = _clipAmmo,
                    reserveAmmo = _reserveAmmo
                }
            );
        }
    }

    // -- Supplemental Methods -- //
    /// <summary>
    /// Designed specifically to register the first weapon. This way there is always data for WeaponSwapped to utilize when switching weapons.
    /// Otherwise there are weird side cases we have to account for in WeaponSwapped().
    /// </summary>
    /// <remarks>This method should be called when a weapon is first equipped or initialized to ensure that
    /// ammunition counts are set correctly. It updates both the internal ammo fields and the ammo state tracking
    /// dictionary.</remarks>
    private void RegisterFirstWeapon()
    {
        int ammoTemp = weaponData.ClipSize;
        _clipAmmo = ammoTemp;
        _maxClipAmmo = ammoTemp;
        _reserveAmmo = weaponData.ReserveAmmo;
        _maxReserveAmmo = weaponData.MaxReserveAmmo;

        ammoStateDictionary.Add(weaponData, new AmmoState
        {
            ammoInClip = _clipAmmo,
            reserveAmmo = _reserveAmmo
        });
    }
    /// <summary>
    /// Initializes error checking for the script.
    /// </summary>
    /// <remarks>We try to catch nulls when we start.</remarks>
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
        if(weaponData == null)
        {
            Debug.LogError("[AmmoHandler] weaponData is empty. It should be something at least.");
        }
    }

    // -- Coroutine -- //

    /// <summary>
    /// Reloads the current gun. Fires off two Unity Events.
    /// </summary>
    private IEnumerator Reload()
    {
        isReloading = true;
        OnReloadingStarting?.Invoke();
        yield return new WaitForSeconds(weaponData.ReloadTime);
        // if we have enough reserve ammo then just fill the clip.
        if (_maxClipAmmo - _clipAmmo <= _reserveAmmo)
        {
            _reserveAmmo -= _maxClipAmmo - _clipAmmo;
            _clipAmmo = _maxClipAmmo;
        }
        // if we don't have enough reserve ammo, use it all.
        else 
        {
            _clipAmmo += _reserveAmmo;
            _reserveAmmo = 0;
        }
        isReloading = false;
        OnReloadingFinished?.Invoke();
    }
}
