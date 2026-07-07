using UnityEngine;
using TMPro;
using System.Collections;

public class SceneDirector_PoolManager : MonoBehaviour
{
    [Header("UI for Pools")]
    [SerializeField] TextMeshProUGUI TestOnePoolSize;
    [SerializeField] TextMeshProUGUI TestOneActiveCount;
    [SerializeField] TextMeshProUGUI TestOneInactiveCount;

    [Header("TestOne Information")]
    [SerializeField] GameObject testOneObject;
    [SerializeField] GameObject testOneSpawnPoint;
    [SerializeField] int testOneQTY = 10;
    [SerializeField] float testOneSpawnsPerSecond = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i <= 1; i++)
        {
            SpawningSoManyThings();
        }
        StartCoroutine(UpdateUICounts());
    }

    void SpawningSoManyThings()
    {
        StartCoroutine(SpawnManager.Instance.Spawn(testOneObject, testOneQTY, testOneSpawnPoint, testOneSpawnsPerSecond));
    }

    void UpdatePoolNumbers(PoolManager.PoolType Pool)
    {
        TestOnePoolSize.text = $"Pool Size: {PoolManager.Instance.GetPoolSize(Pool)}";
        TestOneActiveCount.text = $"Total Active: {PoolManager.Instance.GetActiveCount(Pool)}";    
        TestOneInactiveCount.text = $"Total Inactive: {PoolManager.Instance.GetInactiveCount(Pool)}";
    }

    IEnumerator UpdateUICounts()
    {
        WaitForSeconds wait = new(.25f);
        while (true)
        {
            yield return wait;
            UpdatePoolNumbers(PoolManager.PoolType.PoolManagerTestOne);
        }
    }
}
