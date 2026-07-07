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
    [SerializeField] bool testOnePrewarm = false;
    [SerializeField] int testOnePrewarmQTY = 10;

    void Start()
    {
        if (testOnePrewarm) { PrewarmPool(testOneObject, testOnePrewarmQTY); }
        for (int i = 0; i <= 4; i++)
        {
            SpawningSoManyThings();
        }
        StartCoroutine(UpdateUICounts());
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
