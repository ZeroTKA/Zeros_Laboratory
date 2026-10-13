using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FakeSpeaker_SoundManagerTest : MonoBehaviour
{
    private AudioSource source;
    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        StartCoroutine(ReturnWhenDone());
    }
    private IEnumerator ReturnWhenDone()
    {
        while(!source.isPlaying) yield return null;
        if (source.loop) yield break;

        float pitch = Mathf.Max(Mathf.Abs(source.pitch), 0.01f);
        yield return new WaitForSeconds(source.clip.length / pitch);

        PoolManager.Instance.PutBack(gameObject);
    }
}
