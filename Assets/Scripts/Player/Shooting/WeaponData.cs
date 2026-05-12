using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
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
    [Tooltip("Raycast: Shoots a ray to see if it hits anything. Projectile: places a projectile and lets the projectile do the work. ")]
    [SerializeField] private ShotTypes _shotType;
    [Tooltip("The prefab of the projectile it is going to be using.")]
    [SerializeField] private GameObject _projectilePrefab;

    [Header("Fire Settings")]
    [Tooltip("Semi: One shot per click. Burst: Defined amount per click with a defined delay. Auto: Shoots as much as possible while holding the button.")]
    [SerializeField] private FireModes _availableFireModes;
    [Tooltip("The amount of shots per click.")]
    [SerializeField] private int _burstCount;
    [Tooltip("The time between each shot in the burst count--in seconds.")]
    [SerializeField] private float _burstDelay;
    [Tooltip("How quickly the gun can shoot--in shots per second")]
    [SerializeField] private float _fireRate;
    [Tooltip("The distance for how far the ray travels--in meters")]
    [SerializeField] private float _effectiveRange;

    [Header("Ammo Settings")]
    [Tooltip("The time it takes to reload--in seconds.")]
    [SerializeField] private float _reloadTime;
    [Tooltip("Maximum amount of bullets that can fit into a single clip.")]
    [SerializeField] private int _maxClipAmmo;
    [Tooltip("Carrying capacity of the ammo.")]
    [SerializeField] private int _maxReserveAmmo;
    [Tooltip("The starting amount of reserve ammo.")]
    [SerializeField] private int _reserveAmmo;
    [Tooltip("Minimum base damage")]
    [SerializeField] private float _minBaseDamage;
    [Tooltip("Maximum base damage")]
    [SerializeField] private float _maxBaseDamage;


    public ShotTypes ShotType => _shotType;
    public GameObject ProjectilePrefab => _projectilePrefab;
    public FireModes AvailableFireModes => _availableFireModes;
    public int BurstCount => _burstCount;
    public float BurstDelay => _burstDelay;
    public float FireRate => _fireRate;
    public float EffectiveRange => _effectiveRange;
    public float ReloadTime => _reloadTime;
    public int ClipSize => _maxClipAmmo;
    public int MaxReserveAmmo => _maxReserveAmmo;
    public int ReserveAmmo => _reserveAmmo;
    public float MinDamage => _minBaseDamage;
    public float MaxDamage => _maxBaseDamage;
    private void OnValidate()
    {
        if (_shotType == ShotTypes.Projectile && _projectilePrefab == null) { Debug.LogWarning("[WeaponData] No prefab selected with projectile shooting. Select raycast or pick a prefab."); return; }
        if (_availableFireModes == 0) { Debug.LogWarning("[WeaponData] Fire Mode can not be nothing. It must be something."); return; }
        if (_availableFireModes.HasFlag(FireModes.Burst) && _burstCount < 2) { Debug.LogWarning("[WeaponData] Burst Count must be greater than 1 with fire mode Burst. If you want 1, pick Semi."); return; }
        if (_availableFireModes.HasFlag(FireModes.Burst) && _burstDelay < 0) { Debug.LogWarning("[WeaponData] Burst Delay can not be a negative number with fire mode Burst."); return; }
        if (_fireRate <= 0) { Debug.LogWarning("[WeaponData] Fire Rate must be greater than 0."); return; }
        if (_effectiveRange <= 0) { Debug.LogWarning("[WeaponData] Effective Range must be greater than 0."); return; }
        if (_reloadTime < 0) { Debug.LogWarning("[WeaponData] Reload Time can not be a negative number."); return; }
        if (_maxClipAmmo <= 0) { Debug.LogWarning("[WeaponData] Maximum Clip Ammo must be greater than 0."); return; }
        if (_maxReserveAmmo < 0) { Debug.LogWarning("[WeaponData] Maximum Reserve Ammo can not be a negative number."); return; }
        if (_reserveAmmo > _maxReserveAmmo) { Debug.LogWarning("[WeaponData] Reserve Ammo should not be greater than the Maximum Reserve Ammo."); return; }
        if (_reserveAmmo < 0 ) { Debug.LogWarning("[WeaponData] Reserve Ammo can not be less than 0."); return; }
        if (_minBaseDamage < 0) { Debug.LogWarning("[WeaponData] Minimum Base Damage can not a negative number."); return; }
        if (_minBaseDamage > _maxBaseDamage ) { Debug.LogWarning("[WeaponData] Minimum Base Damage can not be greater than Maximum Base Damage."); return; }
        if (_maxBaseDamage < 0) { Debug.LogWarning("[WeaponData] Maximum Base Damage can not be a negative number."); return; }   
    }

}
