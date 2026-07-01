using UnityEngine;

public class SceneDirector_SpawnManager : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] GameObject enemeyBurst;
    [SerializeField] GameObject enemeyBurstDual;
    [SerializeField] GameObject enemeySpawn;
    [SerializeField] GameObject enemeySpawnDual;
    [SerializeField] GameObject enemeyDuration;
    [SerializeField] GameObject enemeyDurationDual;

    [Header("Spawn Points")]
    [SerializeField] GameObject spawnBurst;
    [SerializeField] GameObject[] spawnBurstDual;
    [SerializeField] GameObject spawn;
    [SerializeField] GameObject[] spawnDual;
    [SerializeField] GameObject spawnDuration;
    [SerializeField] GameObject[] spawnDurationDual;

    

    private void Start()
    {
        StartCoroutine(SpawnManager.Instance.SpawnBurst(enemeyBurst, 100, spawnBurst));
        StartCoroutine(SpawnManager.Instance.SpawnBurst(enemeyBurstDual, 100, spawnBurstDual));

        StartCoroutine(SpawnManager.Instance.Spawn(enemeySpawn, 100, spawn, 15));
        StartCoroutine(SpawnManager.Instance.Spawn(enemeySpawnDual, 100, spawnDual, 15));

        StartCoroutine(SpawnManager.Instance.SpawnByDuration(enemeyDuration, 100, spawnDuration, .2f));
        StartCoroutine(SpawnManager.Instance.SpawnByDuration(enemeyDurationDual, 100, spawnDurationDual, .2f));
    }
}
