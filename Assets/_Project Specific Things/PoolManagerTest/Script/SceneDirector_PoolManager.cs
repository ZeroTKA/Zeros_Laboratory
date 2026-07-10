using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class SceneDirector_PoolManager : MonoBehaviour
{
    /// <summary>
    /// Test 1: Prewarming and if the pools are being created
    /// </summary>

    [Header("UI for Pools")]
    [SerializeField] TextMeshProUGUI TestOnePoolSize;
    [SerializeField] TextMeshProUGUI TestOneActiveCount;
    [SerializeField] TextMeshProUGUI TestOneInactiveCount;

    [Header("TestOne Information")]
    [SerializeField] GameObject testOneObject;
    [SerializeField] GameObject testOneSpawnPoint;
    [SerializeField] int testOneQTY = 10;
    [SerializeField] float testOneSpawnsPerSecond = 5;
    [SerializeField] bool testOnePrewarm = false;
    [SerializeField] int testOnePrewarmQTY = 10;

    [Header("TestThree Information")]
    [SerializeField] GameObject testThreeObject;

    void Start()
    {
        if (testOnePrewarm) { PrewarmPool(testOneObject, testOnePrewarmQTY); }
        for (int i = 0; i <= 2; i++)
        {
            SpawningSoManyThings();
            StartCoroutine(SpawnManager.Instance.Spawn(testThreeObject, testOneQTY, testOneSpawnPoint, testOneSpawnsPerSecond));
        }
        StartCoroutine(UpdateUICounts());
        StartCoroutine(TestTwo());
    }
    void PrewarmPool(GameObject prefab, int Qty)
    {
        PoolManager.Instance.Prewarm(prefab, Qty);
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
    IEnumerator TestTwo()
    {
        yield return new WaitForSeconds(testOneQTY/testOneSpawnsPerSecond + 3);
        PoolManager.Instance.ClearPool(PoolManager.PoolType.PoolManagerTestOne);
        StartCoroutine(TestThree());
    }
    IEnumerator TestThree()
    {
        if (testOnePrewarm) { PrewarmPool(testOneObject, 10); }
        for (int i = 0; i <= 2; i++)
        {
            SpawningSoManyThings();
            StartCoroutine(SpawnManager.Instance.Spawn(testThreeObject, testOneQTY, testOneSpawnPoint, testOneSpawnsPerSecond));
        }
        yield return new WaitForSeconds(testOneQTY / testOneSpawnsPerSecond + 3);
        PoolManager.Instance.ClearAllPools();
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
