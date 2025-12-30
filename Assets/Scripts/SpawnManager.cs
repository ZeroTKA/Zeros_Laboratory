using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    /// <summary>
    /// To Do List:
    /// 
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
    /// <summary>
    /// Gives control on what to spawn, how many, where, and pacing.
    /// </summary>
    /// <param name="enemyPrefab">What prefab are we spawning?</param>
    /// <param name="quantityToSpawn">How many do you want to spawn?</param>
    /// <param name="spawnPoint">where should we spawn it?</param>
    /// <param name="spawnsPerSecond">How quickly do we spawn?</param>
    /// <remarks>This is to spawn a single prefab at a single location. There are more options available.</remarks>
    IEnumerator Spawn(GameObject enemyPrefab, int quantityToSpawn, GameObject spawnPoint, float spawnsPerSecond)
    {
        return Spawn(new[] { enemyPrefab }, quantityToSpawn, new[] { spawnPoint }, spawnsPerSecond);
    }
    /// <summary>
    /// Gives control on what to spawn, how many, where, and pacing.
    /// </summary>
    /// <param name="enemyPrefab">What prefab are we spawning?</param>
    /// <param name="quantityToSpawn">How many do you want to spawn?</param>
    /// <param name="spawnPoint">what locations can we spawn it?</param>
    /// <param name="spawnsPerSecond">How quickly do we spawn?</param>
    /// <remarks>This is to spawn a single prefab at a variety of locations. There are more options available.</remarks>
    IEnumerator Spawn(GameObject enemyPrefab, int quantityToSpawn, IEnumerable<GameObject> spawnPoints, float spawnsPerSecond)
    {
        return Spawn(new[] { enemyPrefab }, quantityToSpawn, spawnPoints, spawnsPerSecond);
    }
    /// <summary>
    /// Gives control on what to spawn, how many, where, and pacing. The key difference is this will RANDOMLY pick a prefab from a set.
    /// </summary>
    /// <param name="enemyPrefab">What prefab are we spawning?</param>
    /// <param name="quantityToSpawn">How many do you want to spawn?</param>
    /// <param name="spawnPoint">where should we spawn it?</param>
    /// <param name="spawnsPerSecond">How quickly do we spawn?</param>
    /// <remarks>This is to spawn a random prefab at a single location. There are more options available. The intention is if you wanted to spawn some pressure minions but don't care which ones gets spawned.</remarks>
    IEnumerator Spawn(IEnumerable<GameObject> enemyPrefabs, int quantityToSpawn, GameObject spawnPoint, float spawnsPerSecond)
    {
        return Spawn(enemyPrefabs, quantityToSpawn, new[] { spawnPoint }, spawnsPerSecond);
    }
    /// <summary>
    /// Gives control on what to spawn, how many, where, and pacing. The key difference is this will RANDOMLY pick a prefab from a set.
    /// </summary>
    /// <param name="enemyPrefab">What prefab are we spawning?</param>
    /// <param name="quantityToSpawn">How many do you want to spawn?</param>
    /// <param name="spawnPoint">where should we spawn it?</param>
    /// <param name="spawnsPerSecond">How quickly do we spawn?</param>
    /// <remarks>This is to spawn a random prefab at a variety of locations. There are more options available. The intention is if you wanted to spawn some pressure minions but don't care which ones gets spawned.</remarks>
    IEnumerator Spawn(IEnumerable<GameObject> enemyPrefabs, int quantityToSpawn, IEnumerable<GameObject> spawnPoints, float spawnsPerSecond)
    {
        // -- All kinds of error checking -- //
        if (enemyPrefabs == null)
        {
            Debug.LogError("[SpawnManager] enemyPrefabs is null.");
            yield break;
        }

        if (spawnPoints == null)
        {
            Debug.LogError("[SpawnManager] spawnPoints is null.");
            yield break;
        }
        if (quantityToSpawn <= 0)
        {
            Debug.LogError("[SpawnManager] Spawn() tried to run with invalid quantity. Quantity to spawn must be greater than zero.");
            yield break;
        }

        if (PoolManager.Instance == null)
        {
            Debug.LogError("[SpawnManager] PoolManager.Instance is null.");
            yield break;
        }

        // -- Prep work for the loop ahead -- //
        var enemyPrefabsList = enemyPrefabs.ToList();
        if (enemyPrefabsList.Count == 0)
        {
            Debug.LogError("[SpawnManager] enemyPrefabs is empty.");
            yield break;
        }

        List<Bounds> boundsList = new();
        GatherBounds(spawnPoints, boundsList);
        if (boundsList.Count == 0)
        {
            Debug.LogWarning("[SpawnManager] No spawn bounds found. Aborting spawn.");
            yield break;
        }

        List<int> validSpawnPointsList = new();

        // -- Spawn everything if spawnsPerSecond <= 0 -- //        
        if (spawnsPerSecond <= 0f)
        {
            VerifyValidSpawnLocations(validSpawnPointsList, boundsList);
            if (validSpawnPointsList.Count == 0)
            {
                Debug.LogWarning("[SpawnManager] No valid spawn point. Aborting spawn.");
                yield break;
            }
            for (int j = 0; j < quantityToSpawn; j++)
            {
                if (enemySpawnCount >= maxEnemies)
                {
                    yield break;
                }
                GameObject enemy = PoolManager.Instance.Rent(enemyPrefabsList[Random.Range(0, enemyPrefabsList.Count)]);
                enemy.transform.position = GetRandomSpawnLocation(validSpawnPointsList, boundsList);
                enemySpawnCount++;
            }
            yield break;
        }

        // -- Prep work for the loop ahead -- //
        float spawnInterval = 1f / spawnsPerSecond;
        int spawned = 0;
        float accumulator = 0f;

        while (spawned < quantityToSpawn)
        {
            // If the manager was destroyed or disabled, stop.
            if (!isActiveAndEnabled) yield break;

            accumulator += Time.deltaTime;
            while (accumulator >= spawnInterval && spawned < quantityToSpawn)
            {
                VerifyValidSpawnLocations(validSpawnPointsList, boundsList);
                if (validSpawnPointsList.Count == 0)
                {
                    Debug.LogWarning("[SpawnManager] No valid spawn point. Waiting .5 seconds to try and spawn.");
                    yield return new WaitForSeconds(0.5f);
                    break;
                }
                if (enemySpawnCount >= maxEnemies)
                {
                    yield break; // stop spawning entirely when cap reached
                }

                GameObject enemy = PoolManager.Instance.Rent(enemyPrefabsList[Random.Range(0, enemyPrefabsList.Count)]);
                enemy.transform.position = GetRandomSpawnLocation(validSpawnPointsList, boundsList);

                enemySpawnCount++;
                spawned++;
                accumulator -= spawnInterval;
            }
            yield return null;
        }
    }

    IEnumerator SpawnByDuration(IEnumerable<GameObject> enemyPrefabs, int quantityToSpawn, IEnumerable<GameObject> spawnPoints, float spawnInterval)
    {
        List<int> validSpawnPointsList = new();
        List<Bounds> boundsList = new();
        List<GameObject> enemyPrefabsList = enemyPrefabs.ToList();
        GatherBounds(spawnPoints, boundsList);

        for (int i = 0; i < quantityToSpawn; i++)
        {
            yield return new WaitForSeconds(spawnInterval);
            VerifyValidSpawnLocations(validSpawnPointsList, boundsList);
            if (enemySpawnCount >= maxEnemies)
            {
                break;
            }
            if (validSpawnPointsList.Count == 0)
            {
                Debug.LogWarning("[SpawnManager] No valid spawn point. Is that really what we want?");
                break;
            }

            GameObject enemy = PoolManager.Instance.Rent(enemyPrefabsList[Random.Range(0, enemyPrefabsList.Count)]);
            enemy.transform.position = GetRandomSpawnLocation(validSpawnPointsList, boundsList);
            enemySpawnCount++;
        }        
    }


    // -- Supplemental Methods -- //
    /// <summary>
    /// This is to pass a single spawn point to get the bounds added to the list.
    /// </summary>
    /// <param name="spawnPoint">We use the box collider attached to an object to determine the spawn location.</param>
    /// <param name="boundsList">This is the list to keep track of all possible spawn locations.</param>
    /// <remarks>Technically we just funnel this into the other GatherBounds() method. This is stritctly for simplicity.</remarks>
    private void GatherBounds(GameObject spawnPoint, List<Bounds> boundsList)
    {
        GatherBounds(new[] { spawnPoint }, boundsList);
    }
    /// <summary>
    /// Adds the spawn point locations to the bounds list.
    /// </summary>
    /// <param name="spawnPoint">We use the box collider attached to an object to determine the spawn location.</param>
    /// <param name="boundsList">This is the list to keep track of all possible spawn locations.</param>
    /// <remarks>This is intended to be as flexible with your system as possible</remarks>
    private void GatherBounds(IEnumerable<GameObject> spawnPoints, List<Bounds> boundsList)
    {
        foreach (var obj in spawnPoints)
        {
            if (obj.TryGetComponent<BoxCollider>(out var box))
            {
                boundsList.Add(box.bounds);
            }
            else
            {
                Debug.LogError($"[SpawnManager] No Box Collider on {obj.name}");
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
        Bounds randomBounds = boundsList[validSpawnPoints[Random.Range(0, validSpawnPoints.Count)]];
        Vector3 spawnLocation = new(
            Random.Range(randomBounds.min.x, randomBounds.max.x),    // X
            randomBounds.min.y,                                      // Y
            Random.Range(randomBounds.min.z, randomBounds.max.z));   // Z
        return spawnLocation;
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
