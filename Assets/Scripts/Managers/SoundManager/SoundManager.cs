using System;
using System.Collections.Generic;
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

    private readonly Dictionary<int, (AudioSource speaker, SoundData.SoundCategory category)> activeSpeakersDictionary = new();
    private int id = 1;
    public GameObject audioSpeakerPrefab;
    private readonly List<int> idsToRemove = new();

    [Tooltip("The amount of active speakers needed before we check for null speakers on PlaySound().")]
    [SerializeField] int dictionarySweepThreshold = 40;

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
    /// PlaySound sound (2D).
    /// </summary>
    /// <param name="data">The SoundData scriptable object that defines the clip, volume, pitch, and loop behavior.</param>
    public SoundHandler PlaySound(SoundData data)
    {
        if (data == null) { Debug.LogError("[SoundManager] PlaySound was called with a null SoundData"); return default; }
        return PlayClip(data, data.GetClip());
    }
    /// <summary>
    /// PlaySound a specific sound (2D). 
    /// </summary>
    /// <param name="data">The SoundData scriptable object that defines the clip, volume, pitch, and loop behavior.</param>
    /// <param name="index">Index of clip in SoundData's clip array.</param>
    public SoundHandler PlaySound(SoundData data, int index)
    {
        if (data == null) { Debug.LogError("[SoundManager] PlaySound was called with a null SoundData"); return default; }
        return PlayClip(data, data.GetClip(index));
    }
    /// <summary>
    /// PlaySound sound at a 3D location.
    /// </summary>
    /// <param name="data">The SoundData scriptable object that defines the clip, volume, pitch, and loop behavior.</param>
    /// <param name="position">The Vector3 for the origin of this 3D sound.</param>
    public SoundHandler PlaySound(SoundData data, Vector3 position)
    {
        if (data == null) { Debug.LogError("[SoundManager] PlaySound was called with a null SoundData"); return default; }
        return PlayClip(data, data.GetClip(), position);
    }
    /// <summary>
    /// PlaySound a specific sound at a 3d location.
    /// </summary>
    /// <param name="data">The SoundData scriptable object that defines the clip, volume, pitch, and loop behavior.</param>
    /// <param name="index">Index of clip in SoundData's clip array.</param>
    /// <param name="position">The Vector3 for the origin of this 3D sound.</param>
    public SoundHandler PlaySound(SoundData data, int index, Vector3 position)
    {
        if (data == null) { Debug.LogError("[SoundManager] PlaySound was called with a null SoundData"); return default; }
        return PlayClip(data, data.GetClip(index), position);
    }
    /// <summary>
    /// PlaySound a sound at a 3d location and follow a transform.
    /// </summary>
    /// <param name="data">The SoundData scriptable object that defines the clip, volume, pitch, and loop behavior.</param>
    /// <param name="toFollow">The transform the sound should become attached to.</param>
    public SoundHandler PlaySound(SoundData data, Transform toFollow)
    {
        if (data == null) { Debug.LogError("[SoundManager] PlaySound was called with a null SoundData"); return default; }
        return PlayClip(data, data.GetClip(), toFollow: toFollow);
    }
    /// <summary>
    /// PlaySound a specific sound at a 3d location and follow a transform.
    /// </summary>
    /// <param name="data">The SoundData scriptable object that defines the clip, volume, pitch, and loop behavior.</param>
    /// <param name="index">Index of clip in SoundData's clip array.</param>
    /// <param name="toFollow">The transform the sound should become attached to.</param>
    public SoundHandler PlaySound(SoundData data, int index, Transform toFollow)
    {
        if (data == null) { Debug.LogError("[SoundManager] PlaySound was called with a null SoundData"); return default; }
        return PlayClip(data, data.GetClip(index), toFollow: toFollow);
    }

    // -- Supplemental Methods -- //
    /// <summary>
    /// This method is specifically to play clips. This will keep the code in a single location. Rephrase it.
    /// </summary>
    /// <param name="data">The SoundData scriptable object that defines the clip, volume, pitch, and loop behavior.</param>
    /// <param name="clip">The audio clip to play.</param>
    private SoundHandler PlayClip(SoundData data, AudioClip clip, Vector3? position = null, Transform toFollow = null)
    {
        if (clip == null) { return default; } // we don't need to log this as it's already logged but we do need to prevent renting objects with null clips.
        if (data.Is3d && !position.HasValue && toFollow == null) { Debug.LogWarning("[SoundManager] PlaySound() was called for a 3d audio but no position nor transform was passed into PlaySound(). Playing as 2D."); }
        if (!data.Is3d && (position.HasValue || toFollow != null)) { Debug.LogWarning("[SoundManager] PlaySound() was called for a 2d audio but a position or a transform was passed into PlaySound(). Playing as 2d."); }
        if (dictionarySweepThreshold <= activeSpeakersDictionary.Count){ dictionarySweep(); } // trying to find nulls in the dictionary.

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
    /// Returns the correct category based on the SoundData information.
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
    /// Basic error checks to make sure things are setup correctly.
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
    public void StopSound(SoundHandler handler)
    {
        if (handler.Id == 0) { Debug.Log("[SoundManager] There's a sound trying to be stopped but SoundHandler was never set."); }

        if (activeSpeakersDictionary.ContainsKey(handler.Id)) { Stop(id: handler.Id); }
        else { Debug.LogWarning($"[SoundManager] {handler.Id} appears to be an invalid Id as it is not in the activeSpeakersDictionary."); }
    }
    public void StopSound(SoundData.SoundCategory category) { Stop(category: category); } 
    public void StopSound() { Stop(); }
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
    /// <summary>
    /// Once we hit more active speakers than expected, we do a quick check and clear out any null speakers.
    /// </summary>
    private void dictionarySweep()
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
}
