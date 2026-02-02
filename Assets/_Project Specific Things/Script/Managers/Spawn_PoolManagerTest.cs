using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_PoolManagerTest : MonoBehaviour
{
    [SerializeField] private List<GameObject> lightsList = new();
    [SerializeField] private List<float> durationBeforeLights = new();
    [SerializeField] private List<AudioClip> audioClips = new();
    [SerializeField] private List<AudioSource> audioSources = new();
    [SerializeField] private AudioClip music;
    [SerializeField] private AudioSource musicSource;

    public static Spawn_PoolManagerTest instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    public void LetTheShowBegin()
    {
        StartCoroutine(ShowTime());
    }
    private IEnumerator ShowTime()
    {
        musicSource.PlayOneShot(music);
        for (int i = 0; i < lightsList.Count - 1; i++)
        {
            yield return new WaitForSeconds(durationBeforeLights[i]);
            lightsList[i].SetActive(!lightsList[i].activeSelf);
            audioSources[i].PlayOneShot(audioClips[i]);

        }
        yield return new WaitForSeconds(1.2f);
        for (int i = 0; i < 2; i++)
        {
            audioSources[0].PlayOneShot(audioClips[6]); // loudspeaker warning siren
            yield return new WaitForSeconds(.9f);
            audioSources[0].PlayOneShot(audioClips[7]); // loudspeaker intruder alert
            yield return new WaitForSeconds(2f);
        }
        yield return new WaitForSeconds(3.1f - 2f);
        lightsList[5].SetActive(!lightsList[5].activeSelf);
        audioSources[0].PlayOneShot(audioClips[5]);
        WaveManagerTest.instance.Wave1();
    }
}
