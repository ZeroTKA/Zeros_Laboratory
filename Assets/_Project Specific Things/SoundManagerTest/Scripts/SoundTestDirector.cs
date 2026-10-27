using System.Collections;
using UnityEngine;

public class SoundTestDirector : MonoBehaviour
{
    [SerializeField] SoundData musicData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(TestGiver());
    }

    void TestOne() // testing SoundManager.Instance.Play()
    {
        SoundManager.Instance.Play(musicData, new Vector3(0,0,0)); // random clip
        //SoundManager.Instance.Play(musicData, 2); // index in range.

        // Things that causes errors.
        SoundManager.Instance.Play(musicData, 99); // index out of range error.

    }

    private IEnumerator TestGiver()
    {
        for (int i = 0; i < 2; i++)
        {
            TestOne();
            yield return new WaitForSeconds(UnityEngine.Random.Range(90, 180));
        }
    }
}
