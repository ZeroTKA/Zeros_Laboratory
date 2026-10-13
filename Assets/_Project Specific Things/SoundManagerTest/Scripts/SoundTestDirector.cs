using UnityEngine;

public class SoundTestDirector : MonoBehaviour
{
    [SerializeField] SoundData musicData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TestOne();
    }

    void TestOne() // testing SoundManager.Instance.Play()
    {
        SoundManager.Instance.Play(musicData);
        // SoundManager.Instance.Play(musicData, 2);

        // Things that causes errors.
        SoundManager.Instance.Play(musicData, 99); // index out of range error.

    }
}
