using UnityEngine;

public class AmmoHandler : MonoBehaviour
{
    private Shooting shooting;
    private int _ammoInClip;
    public int AmmoInClip => _ammoInClip;

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
        shooting.OnShoot -= AShotWasFired;
    }
    // -- Main Methods -- //
    private void AShotWasFired()
    {        
        _ammoInClip -= 1;
        if(_ammoInClip < 0)
        {
            Debug.LogWarning("[AmmoHandler] You can't have a negative number in a clip.");
        }
    }
}
