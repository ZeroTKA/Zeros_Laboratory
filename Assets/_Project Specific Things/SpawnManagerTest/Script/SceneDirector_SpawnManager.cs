using UnityEngine;

public class SceneDirector_SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject enemeyPrefab;
    [SerializeField] GameObject spawnBurst;
    [SerializeField] GameObject[] spawnBurstDual;
    [SerializeField] GameObject spawn;
    [SerializeField] GameObject[] spawnDual;
    [SerializeField] GameObject spawnDuration;
    [SerializeField] GameObject[] spawnDurationDual;

    

    private void Start()
    {
        StartCoroutine(SpawnManager.Instance.SpawnBurst(enemeyPrefab, 100, spawnBurst));
        StartCoroutine(SpawnManager.Instance.SpawnBurst(enemeyPrefab, 100, spawnBurstDual));

        StartCoroutine(SpawnManager.Instance.Spawn(enemeyPrefab, 100, spawn, 15));
        StartCoroutine(SpawnManager.Instance.Spawn(enemeyPrefab, 100, spawnDual, 15));

        StartCoroutine(SpawnManager.Instance.SpawnByDuration(enemeyPrefab, 100, spawnDuration, .2f));
        StartCoroutine(SpawnManager.Instance.SpawnByDuration(enemeyPrefab, 100, spawnDurationDual, .2f));
    }
}
