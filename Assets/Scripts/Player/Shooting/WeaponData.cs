using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{

    // -- To Do -- //
    // Need to add tooltips, especially about fire rate and reload time and burstdelay. Are they in Seconds? Minutes?

    [System.Flags]
    public enum FireModes
    {
        Semi = 1,
        Burst = 2,
        Auto = 4
    }

    public enum ShotTypes
    {
        Raycast,
        Projectile
    }

    [Header("Shoot Settings")]
    [SerializeField] private ShotTypes _shotType;
    [SerializeField] private GameObject _projectilePrefab;

    [Header("Fire Settings")]
    [SerializeField] private FireModes _availableFireModes;
    [SerializeField] private int _burstCount;
    [Tooltip("In seconds")]
    [SerializeField] private float _burstDelay;
    [Tooltip("Shots per second")]
    [SerializeField] private float _fireRate;
    [Tooltip("In meters")]
    [SerializeField] private int _range;

    [Header("Ammo Settings")]
    [SerializeField] private float _reloadTime;
    [SerializeField] private int _maxClipAmmo;
    [SerializeField] private int _maxReserveAmmo;
    [SerializeField] private int _reserveAmmo;

    public ShotTypes ShootType => _shotType;
    public GameObject ProjectilePrefab => _projectilePrefab;
    public FireModes AvailableFireModes => _availableFireModes;
    public int BurstCount => _burstCount;
    public float BurstDelay => _burstDelay;
    public float FireRate => _fireRate;
    public float Range => _range;
    public float ReloadTime => _reloadTime;
    public int ClipSize => _maxClipAmmo;
    public int MaxReserveAmmo => _maxReserveAmmo;
    public int ReserveAmmo => _reserveAmmo;
    private void OnValidate()
    {
        if (_shotType == ShotTypes.Projectile && _projectilePrefab == null) { Debug.LogWarning("[WeaponData] No prefab selected with projectile shooting. Select raycast or pick a prefab."); return; }
        if (_availableFireModes == 0) { Debug.LogWarning("[WeaponData] Fire Mode can not be nothing. It must be something."); return; }
        if (_availableFireModes.HasFlag(FireModes.Burst) && _burstCount < 2) { Debug.LogWarning("[WeaponData] Burst Count must be greater than 1 with fire mode Burst. If you want 1, pick Semi."); return; }
        if (_availableFireModes.HasFlag(FireModes.Burst) && _burstDelay < 0) { Debug.LogWarning("[WeaponData] Burst Delay can not be a negative number with fire mode Burst."); return; }
        if (_fireRate <= 0) { Debug.LogWarning("[WeaponData] Fire Rate must be greater than 0."); return; }
        if (_range <= 0) { Debug.LogWarning("[WeaponData] Range must be greater than 0"); return; }
        if (_reloadTime < 0) { Debug.LogWarning("[WeaponData] Reload Time can not be a negative number."); return; }
        if (_maxClipAmmo <= 0) { Debug.LogWarning("[WeaponData] Clip Size must be greater than 0"); return; }
        if (_maxReserveAmmo < _maxClipAmmo) { Debug.LogWarning("[WeaponData] Total AmmoCapacity must be greater than Clip Size."); return; }
        if (_reserveAmmo < 0 ) { Debug.LogWarning("[WeaponData] Reserve Ammo can not be less than 0"); }
    }

}
