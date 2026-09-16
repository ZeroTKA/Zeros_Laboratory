using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "Scriptable Objects/SoundData", order = 1)]
public class SoundData : ScriptableObject
{
    public enum SoundCategory
    {
        Environment,
        Music,
        SFX,
        UI
    }

    [Header("Clips")]
    [Tooltip("One clip is picked randomly each time the sound plays")]
    [SerializeField] private AudioClip[] _clips;

    [Header("Mix Settings")]
    [Tooltip("Which mixer channel this belongs to.")]
    [SerializeField] private SoundCategory _category;
    [Tooltip("Base volume before any effects applied. 0 to 1")]
    [Range(0f,1f)]
    [SerializeField] private float _volume = 1f;
    [Tooltip("Should the sound loop when played?")]
    [SerializeField] private bool _loop;
    [Tooltip("Minimum random pitch adjustment. Set both min and max to 1 for no variation")]
    [Range(0f, 1f)]
    [SerializeField] private float _minPitchAdjustment = 1f;
    [Tooltip("Maximum random pitch adjustment. Set both min and max to 1 for no variation")]
    [Range(0f, 1f)]
    [SerializeField] private float _maxPitchAdjustment = 1f;

    [Header("Spatial Settings")]
    [Tooltip("Keeps track if this is 2d or 3d.")]
    [SerializeField] private bool _is3D;
    [Tooltip("Minimum distance for full volume in 3d. Ignored if 2d.")]
    [SerializeField] private float _minDistanceFor3D;
    [Tooltip("Maximum distance before the audio is no longer audible.")]
    [SerializeField] private float _maxDistanceFor3D;

    [Header("Throttling")]
    [Tooltip("Minimum time in seconds before this clip can play again. 0 to disable")]
    [SerializeField] private float _cooldownForThrottling = 0f;

    public AudioClip[] Clips => _clips;
    public SoundCategory Category => _category;
    public float Volume => _volume;
    public bool Loop => _loop;
    public float MinPitch => _minPitchAdjustment;
    public float MaxPitch => _maxPitchAdjustment;
    public bool Is3d => _is3D;
    public float MinDistance => _minDistanceFor3D;
    public float MaxDistance => _maxDistanceFor3D;
    public float Cooldown => _cooldownForThrottling;

    /// <summary>
    /// Retrun a random clip from the array. 
    /// </summary>
    /// <remarks>
    /// If you need a specific clip, pass in the int for the index. GetClip(2), for example.
    /// </remarks>
    public AudioClip GetClip()
    {
        if(_clips == null || _clips.Length == 0)
        {
            Debug.LogError($"[SoundData] '{name}' has no sound clip assigned.");
            return null;
        }
        return _clips[Random.Range(0, _clips.Length)];
    }
    public AudioClip GetClip(int index)
    {
        if (_clips == null || _clips.Length == 0)
        {
            Debug.LogError($"[SoundData] '{name}' has no sound clip assigned.");
            return null;
        }
        if(index < 0 || index >= _clips.Length)
        {
            Debug.LogError($"[SoundData] '{name}' GetClip called with an out of range index. Index range is 0 - {_clips.Length - 1}");
            return null;
        }
        return _clips[index];
    }

    private void OnValidate()
    {
        
    }

}
