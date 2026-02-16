using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManagerTest : MonoBehaviour
{

    [SerializeField] List<GameObject> enemyPrefabsList = new();
    [SerializeField] List<GameObject> SpawnPoints = new();
    public static WaveManagerTest instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("[WaveManager] Multiple instances were created. Destroying duplicate instance.");
            Destroy(gameObject);
            return;
        }
    }

    public void Wave1()
    {
        StartCoroutine(SpawnBurst());
        Spawn();
        SpawnByDuration();
    }
    private IEnumerator SpawnBurst()
    {
        for(int i = 0; i < 50; i++)
        {
            StartCoroutine(SpawnManagerTest.instance.SpawnBurst(enemyPrefabsList[0], 20, SpawnPoints[0]));
            yield return new WaitForSeconds(5f);
        }
        yield return null;
    }
    private void Spawn()
    {
        StartCoroutine(SpawnManagerTest.instance.Spawn(enemyPrefabsList[1], 2000, SpawnPoints[1], 8f));
    }
    private void SpawnByDuration()
    {
        StartCoroutine(SpawnManagerTest.instance.SpawnByDuration(enemyPrefabsList[2], 250, SpawnPoints[2], 1f));
        StartCoroutine(SpawnManagerTest.instance.SpawnByDuration(enemyPrefabsList[2], 250, SpawnPoints[3], 1f));
        StartCoroutine(SpawnManagerTest.instance.SpawnByDuration(enemyPrefabsList[2], 250, SpawnPoints[4], 1f));
        StartCoroutine(SpawnManagerTest.instance.SpawnByDuration(enemyPrefabsList[2], 250, SpawnPoints[5], 1f));
    }
}
