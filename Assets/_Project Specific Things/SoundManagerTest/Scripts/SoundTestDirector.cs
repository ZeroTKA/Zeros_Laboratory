using System.Collections;
using UnityEngine;

public class SoundTestDirector : MonoBehaviour
{
    [SerializeField] SoundData musicData;
    private SoundHandler test;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(TestGiver());
    }

    void TestOne() // testing SoundManager.Instance.PlaySound()
    {
        test = SoundManager.Instance.PlaySound(musicData, new Vector3(0,0,0)); // random clip
        //SoundManager.Instance.PlaySound(musicData, 2); // index in range.

        // Things that causes errors.
        SoundManager.Instance.PlaySound(musicData, 99); // index out of range error.

    }

    private IEnumerator TestGiver()
    {
        for (int i = 0; i < 10; i++)
        {
            TestOne();
            yield return new WaitForSeconds(UnityEngine.Random.Range(10, 20));
            SoundManager.Instance.StopSound(musicData.Category);
            
        }
    }
}
