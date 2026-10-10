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
        if (data.Is3d) { Debug.LogWarning("[SoundManager] Play was called for a 2d audio but it's marked 3D. Playing as 2D."); }
        GameObject speakerObject = PoolManager.Instance.Rent(audioSpeakerPrefab);
        AudioSource speaker = speakerObject.GetComponent<AudioSource>();

        // -- Data to assign
        speaker.clip = data.GetClip();
        speaker.volume = data.Volume;
        speaker.pitch = data.GetPitch();
        speaker.loop = data.Loop;
        speaker.spatialBlend = data.Is3d ? 1f : 0f;
        speaker.Play();
    }
    /// <summary>
    /// Play sound at 3d location and forget.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="position"></param>
    public void Play(SoundData data, Vector3 position)
    {

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
}
