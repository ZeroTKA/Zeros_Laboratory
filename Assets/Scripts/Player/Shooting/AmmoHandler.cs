using System;
using System.Collections;
using UnityEngine;

public class AmmoHandler : MonoBehaviour
{
    [SerializeField] WeaponData weaponData;
    private Shooting shooting;
    private int _clipAmmo;
    private int maxClipAmmo;
    private int reserveAmmo;
    private int maxReserveAmmo;

    public int ClipAmmo => _clipAmmo;
    public event Action Reloading;
    public event Action ReloadFail;

    // -- Specialty Methods -- //
    void Awake()
    {
        StartErrorChecking();
    }
    private void OnDisable()
    {
        shooting.OnShoot -= AShotWasFired;
    }
    private void OnEnable()
    {
        if (shooting != null)
        {
            shooting.OnShoot += AShotWasFired; 
        }
    }
    // -- Main Methods -- //
    private void AShotWasFired()
    {
        _clipAmmo -= 1;
        if (_clipAmmo < 0)
        {
            Debug.LogWarning("[AmmoHandler] You can't have a negative number in a clip.");
        }
    }
    public bool CanWeReload()
    {
        if (_clipAmmo == maxClipAmmo) { return false; }
        if (reserveAmmo == 0) { return false; }
        StartCoroutine(Reload());
        return true;
    }

    // -- Supplemental Methods -- //
    private void StartErrorChecking()
    {
        if (TryGetComponent<Shooting>(out var shootComponent))
        {
            shooting = shootComponent;
        }
        else
        {
            Debug.LogError("[AmmoHandler] Unable to find Shooting Component.");
        }
    }
    private IEnumerator Reload()
    {
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
    }
}
