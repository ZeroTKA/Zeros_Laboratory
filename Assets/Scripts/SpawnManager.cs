using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    /// <summary>
    /// To Do List:
    /// 1. What do we do if no valid spawn locations? Pause?
    /// </summary>
    public static SpawnManager instance;
    Transform playerTransform;

    [SerializeField] GameObject[] spawnPoints;
    [SerializeField] float closestDistanceUntilWeStopSpawning;
    [SerializeField] int maxEnemies;

    public int enemySpawnCount = 0;  // the idea is to allow a maximum spawn amount.
    // private List<Bounds> allBoundsList = new();


    // -- Specialty Methods -- //
    /// <summary>
    /// All initialization is done in Awake so that everything is ready before other scripts run.
    /// Other scripts typically use Start(), which executes after Awake(), preventing null reference issues.
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("[SpawnManager] Multiple instances were created. Destroying duplicate instance.");
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    // -- Main Methods -- //

    IEnumerator Spawn(GameObject enemyPrefab, int quantityToSpawn, GameObject spawnPoint, float spawnsPerSecond)
    {
        float spawnAccumulator = 0f;
        List<int> validSpawnPointsList = new();
        List<Bounds> boundsList = new();
        GatherBounds(spawnPoint, boundsList);

        for (int i = 0; i < quantityToSpawn; i++)
        {
            // Wait until there is room to spawn
            while (enemySpawnCount >= maxEnemies)
            {
                yield return null; // wait one frame, check again
            }
            spawnAccumulator += spawnsPerSecond * Time.deltaTime;
            while (spawnAccumulator >= 1f)
            {
                GameObject enemy = PoolManagerTest.Instance.Rent(enemyPrefab);
                VerifyValidSpawnLocations(validSpawnPointsList,boundsList); // refresh the list of valid spawn points
                enemy.transform.position = GetRandomSpawnLocation(validSpawnPointsList, boundsList);
                if (validSpawnPointsList.Count > 0)
                {
                    enemySpawnCount++;
                }
                spawnAccumulator -= 1f;
            }
            yield return null;
        }
    }

    // -- Supplemental Methods -- //
    private void GatherBounds(GameObject spawnPoint, List<Bounds> boundsList)
    {
        if (spawnPoint.TryGetComponent<BoxCollider>(out var box))
        {
            boundsList.Add(box.bounds);
        }
        else
        {
            Debug.LogError($"[SpawnManager] No Box Collider on {spawnPoint.name}");            
        }

    }
    private void GatherBounds(GameObject[] spawnPoint, List<Bounds> boundsList)
    {
        foreach (GameObject genericObject in spawnPoint)
        {
            if (genericObject.TryGetComponent<BoxCollider>(out var box))
            {
                boundsList.Add(box.bounds);
            }
            else
            {
                Debug.LogError($"[SpawnManager] No Box Collider on {genericObject.name}");
            }
        }
    }
    /// <summary>
    /// Calculates the bounds of the box collider to use for spawn location
    /// </summary>
    /// <remarks>
    /// Ultimately we are getting the X range, Y Value, and Z range. We use this box to determine potential spawn locations.
    /// </remarks>
    private Vector3 GetRandomSpawnLocation(List<int> validSpawnPoints, List<Bounds> boundsList)
    {        
        if (validSpawnPoints.Count > 0)
        {
            Bounds randomBounds = boundsList[validSpawnPoints[Random.Range(0, validSpawnPoints.Count)]];
            Vector3 spawnLocation = new Vector3(
                Random.Range(randomBounds.min.x, randomBounds.max.x),    // X
                randomBounds.min.y,                                      // Y
                Random.Range(randomBounds.min.z, randomBounds.max.z));   // Z
            return spawnLocation;
        }
        else
        {
            Debug.LogWarning("[SpawnManager] We are returning Vector3.zero even though there's no valid spawn location. Why isn't there a valid spawn location?");
            return Vector3.zero;
        }

    }
    /// <summary>
    /// This is a conditional check for spawn points we can spawn at.
    /// </summary>
    private void VerifyValidSpawnLocations(List<int> validSpawnPoints, List<Bounds> boundsList)
    {
        validSpawnPoints.Clear();
        for (int i = 0; i < boundsList.Count; i++)
        {
            /// This is where you add conditional logic to determine if the point is valid. If not, continue.
            /// 
            /// 
            /// if it is valid, add it to the validSpawnPoints list. By default, everything is valid.
            validSpawnPoints.Add(i);
        }
    }
}
