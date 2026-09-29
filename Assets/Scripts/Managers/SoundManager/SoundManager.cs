using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

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
    /// Play sound and forget. 2d
    /// </summary>
    /// <param name="data"></param>
    public void Play(SoundData data)
    {

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
