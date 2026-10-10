using UnityEngine;

public class SoundTestDirector : MonoBehaviour
{
    [SerializeField] SoundData musicData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SoundManager.Instance.Play(musicData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
