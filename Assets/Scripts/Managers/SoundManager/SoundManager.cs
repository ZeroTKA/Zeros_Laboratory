using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
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
    /// <param name="data">SoundData defines the clip, volume, pitch, and loop behavior.</param>
    public void Play(SoundData data)
    {
        if (data == null) { Debug.LogError("[SoundManager] Play was called with a null SoundData"); return; }
        PlayClip(data, data.GetClip());
    }
    /// <summary>
    /// Play a specific sound (2D). 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="index"></param>
    public void Play(SoundData data, int index)
    {
        if (data == null) { Debug.LogError("[SoundManager] Play was called with a null SoundData"); return; }
        PlayClip(data, data.GetClip(index));
    }
    /// <summary>
    /// Play sound at a 3D location and forget.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="position"></param>
    public void Play(SoundData data, Vector3 position)
    {
        if (data == null) { Debug.LogError("[SoundManager] Play was called with a null SoundData"); return; }
        PlayClip(data, data.GetClip(), position);
    }
    public void Play(SoundData data, int index, Vector3 position)
    {
        if (data == null) { Debug.LogError("[SoundManager] Play was called with a null SoundData"); return; }
        PlayClip(data, data.GetClip(index), position);
    }
    /// <summary>
    /// Play a sound at a 3d location and followan transform.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="toFollow"></param>
    public void Play(SoundData data, Transform toFollow)
    {

    }

    // -- Supplemental Methods -- //
    /// <summary>
    /// This method is specifically to play clips. This will keep the code in a single location. Rephrase it.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="clip"></param>
    private void PlayClip(SoundData data, AudioClip clip, Vector3? position = null)
    {
        if (clip == null) { return; } // we don't need to log this as it's already logged but we do need to prevent renting objects with null cips.
        if (data.Is3d && !position.HasValue) { Debug.LogWarning("[SoundManager] Play() was called for a 3d audio but no position passed into Play(). Playing as 2D."); }
        if (!data.Is3d && position.HasValue) { Debug.LogWarning("[SoundManager] Play() was called for a 2d audio but a position was passed into Play(). Playing as 2d."); }

        GameObject speakerObject = PoolManager.Instance.Rent(audioSpeakerPrefab);
        AudioSource speaker = speakerObject.GetComponent<AudioSource>();

        // -- Data to assign
        speaker.clip = clip;
        speaker.volume = data.Volume;
        speaker.pitch = data.GetPitch();
        speaker.loop = data.Loop;
        if (position.HasValue && data.Is3d) // if it's 3d
        { 
            speakerObject.transform.position = position.Value; 
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
}
