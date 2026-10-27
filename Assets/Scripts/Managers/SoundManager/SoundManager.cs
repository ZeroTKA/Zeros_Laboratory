using System;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioMixerGroup masterGroup;
    [SerializeField] AudioMixerGroup uiGroup;
    [SerializeField] AudioMixerGroup gameGroup;
    [SerializeField] AudioMixerGroup musicGroup;
    [SerializeField] AudioMixerGroup ambienceGroup;
    [SerializeField] AudioMixerGroup voiceGroup;


    public GameObject audioSpeakerPrefab;

    // -- Specialty Methods -- //
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("[SoundManager] Multiple instances were created. Destroying duplicate instance.");
            Destroy(gameObject);
            return;
        }
    }

    // -- Main Methods -- //
    /// <summary>
    /// Play sound (2D).
    /// </summary>
    /// <param name="data">The SoundData scriptable object that defines the clip, volume, pitch, and loop behavior.</param>
    public void Play(SoundData data)
    {
        if (data == null) { Debug.LogError("[SoundManager] Play was called with a null SoundData"); return; }
        PlayClip(data, data.GetClip());
    }
    /// <summary>
    /// Play a specific sound (2D). 
    /// </summary>
    /// <param name="data">The SoundData scriptable object that defines the clip, volume, pitch, and loop behavior.</param>
    /// <param name="index">Index of clip in SoundData's clip array.</param>
    public void Play(SoundData data, int index)
    {
        if (data == null) { Debug.LogError("[SoundManager] Play was called with a null SoundData"); return; }
        PlayClip(data, data.GetClip(index));
    }
    /// <summary>
    /// Play sound at a 3D location.
    /// </summary>
    /// <param name="data">The SoundData scriptable object that defines the clip, volume, pitch, and loop behavior.</param>
    /// <param name="position">The Vector3 for the origin of this 3D sound.</param>
    public void Play(SoundData data, Vector3 position)
    {
        if (data == null) { Debug.LogError("[SoundManager] Play was called with a null SoundData"); return; }
        PlayClip(data, data.GetClip(), position);
    }
    /// <summary>
    /// Play a specific sound at a 3d location.
    /// </summary>
    /// <param name="data">The SoundData scriptable object that defines the clip, volume, pitch, and loop behavior.</param>
    /// <param name="index">Index of clip in SoundData's clip array.</param>
    /// <param name="position">The Vector3 for the origin of this 3D sound.</param>
    public void Play(SoundData data, int index, Vector3 position)
    {
        if (data == null) { Debug.LogError("[SoundManager] Play was called with a null SoundData"); return; }
        PlayClip(data, data.GetClip(index), position);
    }
    /// <summary>
    /// Play a sound at a 3d location and follow a transform.
    /// </summary>
    /// <param name="data">The SoundData scriptable object that defines the clip, volume, pitch, and loop behavior.</param>
    /// <param name="toFollow">The transform the sound should become attached to.</param>
    public void Play(SoundData data, Transform toFollow)
    {
        if (data == null) { Debug.LogError("[SoundManager] Play was called with a null SoundData"); return; }
        PlayClip(data, data.GetClip(), toFollow: toFollow);
    }
    /// <summary>
    /// Play a specific sound at a 3d location and follow a transform.
    /// </summary>
    /// <param name="data">The SoundData scriptable object that defines the clip, volume, pitch, and loop behavior.</param>
    /// <param name="index">Index of clip in SoundData's clip array.</param>
    /// <param name="toFollow">The transform the sound should become attached to.</param>
    public void Play(SoundData data, int index, Transform toFollow)
    {
        if (data == null) { Debug.LogError("[SoundManager] Play was called with a null SoundData"); return; }
        PlayClip(data, data.GetClip(index), toFollow: toFollow);
    }

    // -- Supplemental Methods -- //
    /// <summary>
    /// This method is specifically to play clips. This will keep the code in a single location. Rephrase it.
    /// </summary>
    /// <param name="data">The SoundData scriptable object that defines the clip, volume, pitch, and loop behavior.</param>
    /// <param name="clip">The audio clip to play.</param>
    private void PlayClip(SoundData data, AudioClip clip, Vector3? position = null, Transform toFollow = null)
    {
        if (clip == null) { return; } // we don't need to log this as it's already logged but we do need to prevent renting objects with null clips.
        if (data.Is3d && !position.HasValue && toFollow == null) { Debug.LogWarning("[SoundManager] Play() was called for a 3d audio but no position nor transform was passed into Play(). Playing as 2D."); }
        if (!data.Is3d && (position.HasValue || toFollow != null)) { Debug.LogWarning("[SoundManager] Play() was called for a 2d audio but a position or a transform was passed into Play(). Playing as 2d."); }

        GameObject speakerObject = PoolManager.Instance.Rent(audioSpeakerPrefab);
        AudioSource speaker = speakerObject.GetComponent<AudioSource>();

        // -- Data to assign
        speaker.clip = clip;
        speaker.volume = data.Volume;
        speaker.pitch = data.GetPitch();
        speaker.loop = data.Loop;
        speaker.outputAudioMixerGroup = GetCategoryGroup(data.Category);
        if ((position.HasValue || toFollow != null) && data.Is3d ) // if it's 3d
        {
            if (toFollow != null)
            {
                speakerObject.transform.SetParent(toFollow, false);
                speakerObject.transform.localPosition = Vector3.zero;
            }
            else
            {
                speakerObject.transform.position = position.Value;
            }
            speaker.spatialBlend = 1f;
            speaker.maxDistance = data.MaxDistance;
            speaker.minDistance = data.MinDistance;
            speaker.rolloffMode = data.RolloffMode;
        }
        else
        {
            speaker.spatialBlend = 0f;
        }
        speaker.Play();
    }
    private AudioMixerGroup GetCategoryGroup(SoundData.SoundCategory category)
    {
        switch (category)
        {
            case SoundData.SoundCategory.Ambience: return ambienceGroup;
            case SoundData.SoundCategory.Game: return gameGroup;
            case SoundData.SoundCategory.Music: return musicGroup;
            case SoundData.SoundCategory.UI: return uiGroup;
            case SoundData.SoundCategory.Voice: return voiceGroup;
            default:
                Debug.LogError($"[SoundManager No mixer group for {category}");
                return null;
        }
    }
}
