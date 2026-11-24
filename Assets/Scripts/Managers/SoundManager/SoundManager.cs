using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [Header("Audio Mixer")]
    [Tooltip("The main Audio Mixer")]
    [SerializeField] AudioMixer audioMixer;
    [Header("Audio Mixer Groups")]
    [Tooltip("This is the master slider")]
    [SerializeField] AudioMixerGroup masterGroup;
    [Tooltip("Mixer group UI sounds are routed to. Controlled by the UI volume slider.")]
    [SerializeField] AudioMixerGroup uiGroup;
    [Tooltip("Mixer group game sounds are routed to. Controlled by the game volume slider.")]
    [SerializeField] AudioMixerGroup gameGroup;
    [Tooltip("Mixer group music sounds are routed to. Controlled by the music volume slider.")]
    [SerializeField] AudioMixerGroup musicGroup;
    [Tooltip("Mixer group ambience sounds are routed to. Controlled by the ambience volume slider.")]
    [SerializeField] AudioMixerGroup ambienceGroup;
    [Tooltip("Mixer group voice sounds are routed to. Controlled by the voice volume slider.")]
    [SerializeField] AudioMixerGroup voiceGroup;
    [Header("Settings")]
    [Tooltip("When the number of active sounds reaches this value, PlaySound() removes entries whose speaker is null")]
    [SerializeField] int dictionaryCleanupThreshold = 40;
    [Tooltip("The prefab to be used as the Audio Source for sounds. Needs an AudioSource")]
    public GameObject audioSpeakerPrefab;
    private readonly Dictionary<int, (AudioSource speaker, SoundData.SoundCategory category)> activeSpeakersDictionary = new();
    private int id = 1;

    private readonly List<int> idsToRemove = new();

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
        StartErrorChecking();
    }

    // -- Main Methods -- //
    /// <summary>
    /// Plays sound (2D).
    /// </summary>
    /// <param name="data">The SoundData scriptable object that defines the category, volume,loop behavior, pitch, 3D settings, and cooldown.</param>
    /// <returns>A <see cref="SoundHandler"/> to pass to <see cref="StopSound(SoundHandler)"/> later. Returns default on failure.</returns>
    public SoundHandler PlaySound(SoundData data)
    {
        if (data == null) { Debug.LogError("[SoundManager] PlaySound was called with a null SoundData"); return default; }
        return PlayClip(data, data.GetClip());
    }
    /// <summary>
    /// Plays a specific sound (2D). 
    /// </summary>
    /// <param name="data">The SoundData scriptable object that defines the category, volume,loop behavior, pitch, 3D settings, and cooldown.</param>
    /// <param name="index">Index of clip in SoundData's clip array.</param>
    /// <returns>A <see cref="SoundHandler"/> to pass to <see cref="StopSound(SoundHandler)"/> later. Returns default on failure.</returns>
    public SoundHandler PlaySound(SoundData data, int index)
    {
        if (data == null) { Debug.LogError("[SoundManager] PlaySound was called with a null SoundData"); return default; }
        return PlayClip(data, data.GetClip(index));
    }
    /// <summary>
    /// Plays sound at a 3D location.
    /// </summary>
    /// <param name="data">The SoundData scriptable object that defines the category, volume,loop behavior, pitch, 3D settings, and cooldown.</param>
    /// <param name="position">The Vector3 for the origin of this 3D sound.</param>
    /// <returns>A <see cref="SoundHandler"/> to pass to <see cref="StopSound(SoundHandler)"/> later. Returns default on failure.</returns>
    public SoundHandler PlaySound(SoundData data, Vector3 position)
    {
        if (data == null) { Debug.LogError("[SoundManager] PlaySound was called with a null SoundData"); return default; }
        return PlayClip(data, data.GetClip(), position);
    }
    /// <summary>
    /// Plays a specific sound at a 3d location.
    /// </summary>
    /// <param name="data">The SoundData scriptable object that defines the category, volume,loop behavior, pitch, 3D settings, and cooldown.</param>
    /// <param name="index">Index of clip in SoundData's clip array.</param>
    /// <param name="position">The Vector3 for the origin of this 3D sound.</param>
    /// <returns>A <see cref="SoundHandler"/> to pass to <see cref="StopSound(SoundHandler)"/> later. Returns default on failure.</returns>
    public SoundHandler PlaySound(SoundData data, int index, Vector3 position)
    {
        if (data == null) { Debug.LogError("[SoundManager] PlaySound was called with a null SoundData"); return default; }
        return PlayClip(data, data.GetClip(index), position);
    }
    /// <summary>
    /// Plays a sound at a 3d location and follow a transform.
    /// </summary>
    /// <param name="data">The SoundData scriptable object that defines the category, volume,loop behavior, pitch, 3D settings, and cooldown.</param>
    /// <param name="toFollow">The transform the sound should become paranted to.</param>
    /// <returns>A <see cref="SoundHandler"/> to pass to <see cref="StopSound(SoundHandler)"/> later. Returns default on failure.</returns>
    public SoundHandler PlaySound(SoundData data, Transform toFollow)
    {
        if (data == null) { Debug.LogError("[SoundManager] PlaySound was called with a null SoundData"); return default; }
        return PlayClip(data, data.GetClip(), toFollow: toFollow);
    }
    /// <summary>
    /// Plays a specific sound at a 3d location and follow a transform.
    /// </summary>
    /// <param name="data">The SoundData scriptable object that defines the category, volume,loop behavior, pitch, 3D settings, and cooldown.</param>
    /// <param name="index">Index of clip in SoundData's clip array.</param>
    /// <param name="toFollow">The transform the sound should become paranted to.</param>
    /// <returns>A <see cref="SoundHandler"/> to pass to <see cref="StopSound(SoundHandler)"/> later. Returns default on failure.</returns>
    public SoundHandler PlaySound(SoundData data, int index, Transform toFollow)
    {
        if (data == null) { Debug.LogError("[SoundManager] PlaySound was called with a null SoundData"); return default; }
        return PlayClip(data, data.GetClip(index), toFollow: toFollow);
    }
    /// <summary>
    /// Stops a specific sound.
    /// </summary>
    /// <param name="handler">The struct which contains the ID for the sound.</param>
    public void StopSound(SoundHandler handler)
    {
        if (handler.Id == 0) { Debug.Log("[SoundManager] There's a sound trying to be stopped but SoundHandler was never set."); }

        if (activeSpeakersDictionary.ContainsKey(handler.Id)) { Stop(id: handler.Id); }
        else { Debug.LogWarning($"[SoundManager] {handler.Id} appears to be an invalid Id as it is not in the activeSpeakersDictionary. Was it already stopped?"); }
    }
    /// <summary>
    /// Stops all sounds belonging to a specific category.
    /// </summary>
    /// <param name="category">The group of sounds we want to stop.</param>
    public void StopSound(SoundData.SoundCategory category) { Stop(category: category); }
    /// <summary>
    /// Stops all currently playing sounds started through SoundManager.
    /// </summary>
    public void StopSound() { Stop(); }

    // -- Supplemental Methods -- //
    /// <summary>
    /// Removes entries with null speakers from the active speakers dictionary.
    /// </summary>
    private void dictionaryCleanup()
    {
        idsToRemove.Clear();
        foreach (var pair in activeSpeakersDictionary)
        {
            if (pair.Value.speaker == null)
            {
                Debug.LogWarning("[SoundManager] The speaker is null and was removed from the dictionary activeSpeakerDictionary. You should find out how that happened.");
                idsToRemove.Add(pair.Key);
            }
        }
        for (int i = 0; i < idsToRemove.Count; i++)
        {
            activeSpeakersDictionary.Remove(idsToRemove[i]);
        }
    }
    /// <summary>
    /// Returns the mixer group for the given sound category.
    /// </summary>
    /// <param name="category">The assigned category in the SoundData file.</param>
    /// <returns>Audio Mixer Group</returns>
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
    /// <summary>
    /// This is designed to be the single location for the logic for playing sound.
    /// </summary>
    /// <param name="data">The SoundData scriptable object that defines the clip, volume, pitch, and loop behavior.</param>
    /// <param name="clip">The audio clip to play.</param>
    /// <param name="position">The Vector3 for the origin of this 3D sound.</param>
    /// <param name="toFollow">The transform the sound should become paranted to.</param>
    /// <returns>A <see cref="SoundHandler"/> to pass to <see cref="StopSound(SoundHandler)"/> later. Returns default on failure.</returns>
    private SoundHandler PlayClip(SoundData data, AudioClip clip, Vector3? position = null, Transform toFollow = null)
    {
        if (clip == null) { return default; } // we don't need to log this as it's already logged but we do need to prevent renting objects with null clips.
        if (data.Is3d && !position.HasValue && toFollow == null) { Debug.LogWarning("[SoundManager] PlaySound() was called for a 3d audio but no position nor transform was passed into PlaySound(). Playing as 2D."); }
        if (!data.Is3d && (position.HasValue || toFollow != null)) { Debug.LogWarning("[SoundManager] PlaySound() was called for a 2d audio but a position or a transform was passed into PlaySound(). Playing as 2d."); }
        if (activeSpeakersDictionary.Count >= dictionaryCleanupThreshold){ dictionaryCleanup(); } // trying to find nulls in the dictionary.

        GameObject speakerObject = PoolManager.Instance.Rent(audioSpeakerPrefab);
        AudioSource speaker = speakerObject.GetComponent<AudioSource>();
        SoundHandler handler = new(id);
        activeSpeakersDictionary.Add(handler.Id, (speaker, data.Category));
        id++;

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
        return handler;
    }

    /// <summary>
    /// Basic error checks to make sure things are set up correctly.
    /// </summary>
    private void StartErrorChecking()
    {
        if (audioMixer == null) { Debug.LogError("[SoundManager] The aduioMixer is null. Assign something to it in the inspector."); }
        if (ambienceGroup == null) { Debug.LogError("[SoundManager] The ambienceGroup is null. Assign something to it in the inspector."); }
        if (gameGroup == null) { Debug.LogError("[SoundManager] The gameGroup is null. Assign something to it in the inspector."); }
        if (musicGroup == null) { Debug.LogError("[SoundManager] The musicGroup is null. Assign something to it in the inspector."); }
        if (uiGroup == null) { Debug.LogError("[SoundManager] The uiGroup is null. Assign something to it in the inspector."); }
        if (voiceGroup == null) { Debug.LogError("[SoundManager] The voiceGroup is null. Assign something to it in the inspector."); }
    }
    /// <summary>
    /// This is designed to be the single location for the logic for stopping sound.
    /// Stops tracked sounds that match the given filters and removes them from the active speakers dictionary.
    /// </summary>
    /// <param name="id">If passed in, will stop the specific sound associated with this ID.</param>
    /// <param name="category">If passed in, will stop all sounds for this category.</param>
    /// <remarks>
    /// Filters are optional and combine: pass either one, both, or neither.
    /// With one filter, every sound matching it is stopped. With both, a sound must match both.
    /// With neither, every tracked sound is stopped.
    /// Entries with null speakers are removed along the way.
    /// </remarks>
    private void Stop(int? id = null, SoundData.SoundCategory? category = null)
    {
        idsToRemove.Clear();
        foreach (var pair in activeSpeakersDictionary)
        {
            if (pair.Value.speaker == null)
            {
                Debug.LogWarning("[SoundManager] The speaker is null and was removed from the dictionary activeSpeakerDictionary. You should find out how that happened.");
                idsToRemove.Add(pair.Key);
                continue;
            }
            if (id.HasValue && pair.Key != id.Value) continue; // Looks for the ID to do a StopSoundByID type thing.
            if (category.HasValue && pair.Value.category != category.Value) continue; // Looks for the category to do a StopSoundByCategory type thing.

            pair.Value.speaker.Stop();
            idsToRemove.Add(pair.Key);
        }
        Debug.Log($"Before: {activeSpeakersDictionary.Count}");
        for (int i = 0; i < idsToRemove.Count; i++)
        {
            activeSpeakersDictionary.Remove(idsToRemove[i]);
        }
        Debug.Log($"After: {activeSpeakersDictionary.Count}");
    }

}
