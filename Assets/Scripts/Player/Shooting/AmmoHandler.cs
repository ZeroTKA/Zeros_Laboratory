using System;
using UnityEngine;

public class AmmoHandler : MonoBehaviour
{
    private Shooting shooting;
    private int _clipAmmo;
    private int maxClipAmmo;
    private int reserveAmmo;
    private int maxReserveAmmo;

    public int ClipAmmo => _clipAmmo;
    public event Action Reloading;
    public event Action ReloadFail;

    // -- Specialty Methods -- //
    private void Start()
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
    private void OnDisable()
    {
        shooting.OnShoot -= AShotWasFired;
    }
    private void OnEnable()
    {
        shooting.OnShoot += AShotWasFired;
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
        Reload();
        return true;
    }
    private void Reload()
    {
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
