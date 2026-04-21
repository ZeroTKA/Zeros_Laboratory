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
        Auto = 2,
        Burst = 4
    }

    public enum ShootTypes
    {
        Raycast,
        Projectile
    }

    [SerializeField] private FireModes _availableFireModes;
    [SerializeField] private ShootTypes _shootType;
    [SerializeField] private float _fireRate;
    [SerializeField] private int _burstCount;
    [SerializeField] private float _burstDelay;
    [SerializeField] private float _reloadTime;
    [SerializeField] private int _clipSize;
    [SerializeField] private int _totalAmmoCapacity;
    [SerializeField] private GameObject _projectilePrefab;

    public FireModes AvailableFireModes => _availableFireModes;
    public ShootTypes ShootType => _shootType;
    public float FireRate => _fireRate;
    public int BurstCount => _burstCount;
    public float BurstDelay => _burstDelay;
    public float ReloadTime => _reloadTime;
    public int ClipSize => _clipSize;
    public int TotalAmmoCapacity => _totalAmmoCapacity;
    public GameObject ProjectilePrefab => _projectilePrefab;
}
