using System.Collections.Generic;
using UnityEngine;

public class WaveManagerTest : MonoBehaviour
{
    [SerializeField] List<GameObject> enemyPrefabsList = new();
    [SerializeField] List<GameObject> SpawnPoints = new();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnManagerTest.instance.Spawn(enemyPrefabsList, 50000, SpawnPoints,60));
    }
}
