using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManagerTest : MonoBehaviour
{
    /// <summary>
    /// To Do List:
    /// summary / how to use SpawnManager.
    /// </summary>
    public static SpawnManagerTest instance;
    [SerializeField] private WaitForSeconds _WaitForSeconds = new(.5f); // duration we wait incase there are no valid spawns.
    [SerializeField] int maxEnemies;
    private int _enemySpawnCount = 0;
    public int EnemySpawnCount => _enemySpawnCount;

    public void RegisterSpawn() => _enemySpawnCount++;
    public void UnregisterSpawn() => _enemySpawnCount = Mathf.Max(0, _enemySpawnCount - 1);

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

    // -- Main Methods -- //

        // -- SpawnBurst() -- //
    /// <summary>
    /// Spawns a set number of enemies at specified spawn points immediately.
    /// </summary>
    /// <param name="enemyPrefab">The prefab to spawn.</param>
    /// <param name="quantityToSpawn">The total number of enemies to spawn.</param>
    /// <param name="spawnPoint">The spawn point object used to determine a valid spawn location.</param>
    public IEnumerator SpawnBurst(GameObject enemyPrefab, int quantityToSpawn, GameObject spawnPoint)
    {
        return SpawnBurst(new[] { enemyPrefab }, quantityToSpawn, new[] { spawnPoint });
    }
    /// <summary>
    /// Spawns a set number of enemies at specified spawn points immediately.
    /// </summary>
    /// <param name="enemyPrefab">The prefab to spawn.</param>
    /// <param name="quantityToSpawn">The total number of enemies to spawn.</param>
    /// <param name="spawnPoints">The collection of spawn point objects used to determine valid spawn locations.</param>
    public IEnumerator SpawnBurst(GameObject enemyPrefab, int quantityToSpawn, IEnumerable<GameObject> spawnPoints)
    {
        return SpawnBurst(new[] { enemyPrefab }, quantityToSpawn, spawnPoints);
    }
    /// <summary>
    /// Spawns a set number of enemies at specified spawn points immediately.
    /// </summary>
    /// <param name="enemyPrefabs">The list of possible enemy prefabs to spawn. One is chosen at random each time.</param>
    /// <param name="quantityToSpawn">The total number of enemies to spawn.</param>
    /// <param name="spawnPoint">The spawn point object used to determine a valid spawn location.</param>
    public IEnumerator SpawnBurst(IEnumerable<GameObject> enemyPrefabs, int quantityToSpawn, GameObject spawnPoint)
    {
        return SpawnBurst(enemyPrefabs, quantityToSpawn, new[] { spawnPoint });
    }
    /// <summary>
    /// Spawns a set number of enemies at specified spawn points immediately.
    /// </summary>
    /// <param name="enemyPrefabs">The list of possible enemy prefabs to spawn. One is chosen at random each time.</param>
    /// <param name="quantityToSpawn">The total number of enemies to spawn.</param>
    /// <param name="spawnPoints">The collection of spawn point objects used to determine valid spawn locations.</param>
    public IEnumerator SpawnBurst(IEnumerable<GameObject> enemyPrefabs, int quantityToSpawn, IEnumerable<GameObject> spawnPoints)
    {
        // -- All kinds of error checking -- //
        if (enemyPrefabs == null)
        {
            Debug.LogError("[SpawnManager] enemyPrefabs is null.");
            yield break;
        }

        if (quantityToSpawn <= 0)
        {
            Debug.LogError("[SpawnManager] Spawn() tried to run with invalid quantity. Quantity to spawn must be greater than zero.");
            yield break;
        }

        if (spawnPoints == null)
        {
            Debug.LogError("[SpawnManager] spawnPoints is null.");
            yield break;
        }

        if (PoolManagerTest.Instance == null)
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
        VerifyValidSpawnLocations(validSpawnPointsList, boundsList);
        if (validSpawnPointsList.Count == 0)
        {
            Debug.LogWarning("[SpawnManager] No valid spawn point. Aborting spawn.");
            yield break;
        }
        for (int j = 0; j < quantityToSpawn; j++)
        {            
            if (_enemySpawnCount >= maxEnemies)
            {
                yield break;
            }
            GameObject enemy = PoolManagerTest.Instance.Rent(enemyPrefabsList[Random.Range(0, enemyPrefabsList.Count)]);
            enemy.transform.position = GetRandomSpawnLocation(validSpawnPointsList, boundsList);
            RegisterSpawn();
        }
    }

        // -- Spawn() -- //
    /// <summary>
    /// Spawns a set number of enemies at specified spawn points, using an amount we want to spawn each second.
    /// </summary>
    /// <param name="enemyPrefab">The prefab to spawn.</param>
    /// <param name="quantityToSpawn">The grand total number of enemies to spawn.</param>
    /// <param name="spawnPoint">The spawn point object used to determine a valid spawn location.</param>
    /// <param name="spawnsPerSecond">The number of spawns we do each second.</param>
    public IEnumerator Spawn(GameObject enemyPrefab, int quantityToSpawn, GameObject spawnPoint, float spawnsPerSecond)
    {
        return Spawn(new[] { enemyPrefab }, quantityToSpawn, new[] { spawnPoint }, spawnsPerSecond);
    }
    /// <summary>
    /// Spawns a set number of enemies at specified spawn points, using an amount we want to spawn each second.
    /// </summary>
    /// <param name="enemyPrefab">The prefab to spawn.</param>
    /// <param name="quantityToSpawn">The grand total number of enemies to spawn.</param>
    /// <param name="spawnPoints">The collection of spawn point objects used to determine valid spawn locations.</param>
    /// <param name="spawnsPerSecond">The number of spawns we do each second.</param>
    public IEnumerator Spawn(GameObject enemyPrefab, int quantityToSpawn, IEnumerable<GameObject> spawnPoints, float spawnsPerSecond)
    {
        return Spawn(new[] { enemyPrefab }, quantityToSpawn, spawnPoints, spawnsPerSecond);
    }
    /// <summary>
    /// Spawns a set number of enemies at specified spawn points, using an amount we want to spawn each second.
    /// </summary>
    /// <param name="enemyPrefabs">The list of possible enemy prefabs to spawn. One is chosen at random each time.</param>
    /// <param name="quantityToSpawn">The grand total number of enemies to spawn.</param>
    /// <param name="spawnPoint">The spawn point object used to determine a valid spawn location.</param>
    /// <param name="spawnsPerSecond">The number of spawns we do each second.</param>
    public IEnumerator Spawn(IEnumerable<GameObject> enemyPrefabs, int quantityToSpawn, GameObject spawnPoint, float spawnsPerSecond)
    {
        return Spawn(enemyPrefabs, quantityToSpawn, new[] { spawnPoint }, spawnsPerSecond);
    }
    /// <summary>
    /// Spawns a set number of enemies at specified spawn points, using an amount we want to spawn each second.
    /// </summary>
    /// <param name="enemyPrefabs">The list of possible enemy prefabs to spawn. One is chosen at random each time.</param>
    /// <param name="quantityToSpawn">The grand total number of enemies to spawn.</param>
    /// <param name="spawnPoints">The collection of spawn point objects used to determine valid spawn locations.</param>
    /// <param name="spawnsPerSecond">The number of spawns we do each second.</param>
    public IEnumerator Spawn(IEnumerable<GameObject> enemyPrefabs, int quantityToSpawn, IEnumerable<GameObject> spawnPoints, float spawnsPerSecond)
    {
        // -- All kinds of error checking -- //
        if (enemyPrefabs == null)
        {
            Debug.LogError("[SpawnManager] enemyPrefabs is null.");
            yield break;
        }

        if (quantityToSpawn <= 0)
        {
            Debug.LogError("[SpawnManager] Spawn() tried to run with invalid quantity. Quantity to spawn must be greater than zero.");
            yield break;
        }

        if (spawnPoints == null)
        {
            Debug.LogError("[SpawnManager] spawnPoints is null.");
            yield break;
        }

        if (spawnsPerSecond <= 0)
        {
            Debug.LogError("[SpawnManager] spawnsPerSecond is an invalid number. It must be greater than 0");
            yield break;
        }

        if (PoolManagerTest.Instance == null)
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
                    Debug.LogWarning("[SpawnManager] No valid spawn point. Waiting to try and spawn.");
                    yield return _WaitForSeconds;
                    break;
                }
                if (_enemySpawnCount >= maxEnemies)
                {
                    yield break; // stop spawning entirely when cap reached
                }

                GameObject enemy = PoolManagerTest.Instance.Rent(enemyPrefabsList[Random.Range(0, enemyPrefabsList.Count)]);
                enemy.transform.position = GetRandomSpawnLocation(validSpawnPointsList, boundsList);

                RegisterSpawn();
                spawned++;
                accumulator -= spawnInterval;
            }
            yield return null;
        }
    }

        // -- SpawnByDuration() -- //
    /// <summary>
    /// Spawns a set number of enemies at specified spawn points, using a fixed time interval between each spawn.
    /// </summary>
    /// <param name="enemyPrefab">The prefab to spawn.</param>
    /// <param name="quantityToSpawn">The total number of enemies to spawn.</param>
    /// <param name="spawnPoint">The spawn point object used to determine a valid spawn location.</param>
    /// <param name="spawnInterval">The time (in seconds) to wait between each spawn attempt.</param>
    public IEnumerator SpawnByDuration(GameObject enemyPrefab, int quantityToSpawn, GameObject spawnPoint, float spawnInterval)
    {
        return SpawnByDuration(new[] { enemyPrefab }, quantityToSpawn, new[] { spawnPoint }, spawnInterval);
    }
    /// <summary>
    /// Spawns a set number of enemies at specified spawn points, using a fixed time interval between each spawn.
    /// </summary>
    /// <param name="enemyPrefab">The prefab to spawn.</param>
    /// <param name="quantityToSpawn">The total number of enemies to spawn.</param>
    /// <param name="spawnPoints">The collection of spawn point objects used to determine valid spawn locations.</param>
    /// <param name="spawnInterval">The time (in seconds) to wait between each spawn attempt.</param>
    public IEnumerator SpawnByDuration(GameObject enemyPrefab, int quantityToSpawn, IEnumerable<GameObject> spawnPoints, float spawnInterval)
    {
        return SpawnByDuration(new[] { enemyPrefab }, quantityToSpawn, spawnPoints, spawnInterval);
    }
    /// <summary>
    /// Spawns a set number of enemies at specified spawn points, using a fixed time interval between each spawn.
    /// </summary>
    /// <param name="enemyPrefabs">The list of possible enemy prefabs to spawn. One is chosen at random each time.</param>
    /// <param name="quantityToSpawn">The total number of enemies to spawn.</param>
    /// <param name="spawnPoint">The spawn point object used to determine a valid spawn location.</param>
    /// <param name="spawnInterval">The time (in seconds) to wait between each spawn attempt.</param>
    public IEnumerator SpawnByDuration(IEnumerable<GameObject> enemyPrefabs, int quantityToSpawn, GameObject spawnPoint, float spawnInterval)
    {
        return SpawnByDuration(enemyPrefabs, quantityToSpawn, new[] { spawnPoint }, spawnInterval);
    }
    /// <summary>
    /// Spawns a set number of enemies at specified spawn points, using a fixed time interval between each spawn.
    /// </summary>
    /// <param name="enemyPrefabs">The list of possible enemy prefabs to spawn. One is chosen at random each time.</param>
    /// <param name="quantityToSpawn">The total number of enemies to spawn.</param>
    /// <param name="spawnPoints">The collection of spawn point objects used to determine valid spawn locations.</param>
    /// <param name="spawnInterval">The time (in seconds) to wait between each spawn attempt.</param>
    public IEnumerator SpawnByDuration(IEnumerable<GameObject> enemyPrefabs, int quantityToSpawn, IEnumerable<GameObject> spawnPoints, float spawnInterval)
    {
        Debug.Log("Made it here");
        // -- All kinds of error checking -- //
        if (enemyPrefabs == null)
        {
            Debug.LogError("[SpawnManager] enemyPrefabs is null.");
            yield break;
        }

        if (quantityToSpawn <= 0)
        {
            Debug.LogError("[SpawnManager] Spawn() tried to run with invalid quantity. Quantity to spawn must be greater than zero.");
            yield break;
        }

        if (spawnPoints == null)
        {
            Debug.LogError("[SpawnManager] spawnPoints is null.");
            yield break;
        }

        if (spawnInterval <= 0f)
        {
            Debug.LogError("[SpawnManager] spawnInterval must be > 0");
            yield break;
        }

        if (PoolManagerTest.Instance == null)
        {
            Debug.LogError("[SpawnManager] PoolManagerTest.Instance is null.");
            yield break;
        }

        // -- Prep work for the loop below -- //
        List<int> validSpawnPointsList = new();
        List<Bounds> boundsList = new();
        GatherBounds(spawnPoints, boundsList);
        if (boundsList.Count == 0)
        {
            Debug.LogWarning("[SpawnManager] No spawn bounds found. Aborting spawn.");
            yield break;
        }

        List<GameObject> enemyPrefabsList = enemyPrefabs.ToList();
        if (enemyPrefabsList.Count == 0)
        {
            Debug.LogError("[SpawnManager] enemyPrefabs is empty.");
            yield break;
        }

        for (int i = 0; i < quantityToSpawn; i++)
        {
            if (!isActiveAndEnabled) yield break;
            yield return new WaitForSeconds(spawnInterval);
            VerifyValidSpawnLocations(validSpawnPointsList, boundsList);
            if (_enemySpawnCount >= maxEnemies)
            {
                break;
            }
            if (validSpawnPointsList.Count == 0)
            {
                Debug.LogWarning("[SpawnManager] No valid spawn point. Is that really what we want?");
                break;
            }

            GameObject enemy = PoolManagerTest.Instance.Rent(enemyPrefabsList[Random.Range(0, enemyPrefabsList.Count)]);
            enemy.transform.position = GetRandomSpawnLocation(validSpawnPointsList, boundsList);
            RegisterSpawn();
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
    /// <param name="spawnPoints">We use the box collider attached to an object to determine the spawn location.</param>
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
