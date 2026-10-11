using UnityEngine;

public class SoundTestDirector : MonoBehaviour
{
    [SerializeField] SoundData musicData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SoundManager.Instance.Play(musicData);
        SoundManager.Instance.Play(musicData, 8);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
