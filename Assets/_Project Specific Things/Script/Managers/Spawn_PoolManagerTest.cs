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
        for (int i = 0; i < lightsList.Count; i++)
        {
            yield return new WaitForSeconds(durationBeforeLights[i]);
            lightsList[i].SetActive(!lightsList[i].activeSelf);
            audioSources[i].PlayOneShot(audioClips[i]);
        }
        WaveManagerTest.instance.Wave1();
    }
}
