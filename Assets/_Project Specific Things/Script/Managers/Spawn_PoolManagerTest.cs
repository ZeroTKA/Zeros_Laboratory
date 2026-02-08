using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Spawn_PoolManagerTest : MonoBehaviour
{
    [Header("Sound Things")]
    [SerializeField] private List<GameObject> lightsList = new();
    [SerializeField] private List<float> durationBeforeLights = new();
    [SerializeField] private List<AudioClip> audioClips = new();
    [SerializeField] private List<AudioSource> audioSources = new();
    [SerializeField] private AudioClip music;
    [SerializeField] private AudioSource musicSource;

    [Header("Loading Screen")]
    [SerializeField] TextMeshProUGUI loadingText;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] GameObject logo;
    private bool stopLoading = false;

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
        StartCoroutine(Logo());
        // Turn on main lights
        musicSource.PlayOneShot(music);
        for (int i = 0; i < lightsList.Count - 1; i++)
        {
            yield return new WaitForSeconds(durationBeforeLights[i]);
            lightsList[i].SetActive(!lightsList[i].activeSelf);
            audioSources[i].PlayOneShot(audioClips[i]);

        }
        yield return new WaitForSeconds(1.2f);

        // do loudspeaker things.
        for (int i = 0; i < 2; i++)
        {
            audioSources[0].PlayOneShot(audioClips[6]); // loudspeaker warning siren
            yield return new WaitForSeconds(.9f);
            audioSources[0].PlayOneShot(audioClips[7]); // loudspeaker intruder alert
            yield return new WaitForSeconds(2f);
        }

        // turn on the last set of lights and activate bad guys.

        yield return new WaitForSeconds(3.1f - 2f);
        stopLoading = true;
        loadingText.gameObject.SetActive(false);
        lightsList[5].SetActive(!lightsList[5].activeSelf);
        audioSources[0].PlayOneShot(audioClips[5]);
        WaveManagerTest.instance.Wave1();
    }

    private IEnumerator AnimateLoadingTextEllipsis()
    {        
        loadingScreen.SetActive(true);
        while(!stopLoading)
        {
            WaitForSeconds waiting = new(1.2f);
            // we assume the text ends in . . .. This means we animate the '. . .' String can be any size.
            for (int i = 0; i < 4; i++)
            {
                switch(i)
                {
                    case 0:
                        loadingText.maxVisibleCharacters = loadingText.text.Length - 6;
                        break;
                    case 1:
                        loadingText.maxVisibleCharacters = loadingText.text.Length - 4;
                        break;
                    case 2:
                        loadingText.maxVisibleCharacters = loadingText.text.Length - 2;
                        break;
                    case 3:
                        loadingText.maxVisibleCharacters = loadingText.text.Length;
                        break;
                }
                yield return waiting;
            }
        }
    }
    private IEnumerator Logo()
    {
        yield return new WaitForSeconds(3.8f);
        logo.SetActive(true);
        yield return new WaitForSeconds(3f);
        logo.SetActive(false);
        StartCoroutine(AnimateLoadingTextEllipsis());
       

    }
}
