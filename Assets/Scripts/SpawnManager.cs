using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    /// <summary>
    /// To Do
    /// 3. What do we do if no valid spawn locations? Pause?
    /// </summary>
    public static SpawnManager instance;
    Transform playerTransform;

    [SerializeField] GameObject[] prefab; // Temp Testing, Remove later.
    [SerializeField] GameObject[] spawnPoints;
    [SerializeField] float closestDistanceUntilWeStopSpawning;
    [SerializeField] int maxEnemies;
    [SerializeField] int TotalEnemiesToSpawn;

    public int enemySpawnCount = 0;  // the idea is to allow a maximum spawn amount.
    private List<Bounds> allBoundsList = new();
    private List<int> indexOfValidSpawnPoints = new();

    // -- Specialty Methods -- //
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
        StartCoroutine(Spawn());
    }

    // -- Main Method -- //


    // -- Supplemental Methods -- //

    private void GatherBoundsForList()
    {
        foreach (GameObject genericObject in spawnPoints)
        {
            if (genericObject.TryGetComponent<BoxCollider>(out var box))
            {
                allBoundsList.Add(box.bounds);
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
    private Vector3 GetRandomSpawnLocation()
    {
        GetValidSpawnLocations();
        if (indexOfValidSpawnPoints.Count > 0)
        {
            Bounds randomBounds = allBoundsList[indexOfValidSpawnPoints[Random.Range(0, indexOfValidSpawnPoints.Count)]];
            Vector3 spawnLocation = new Vector3(
                Random.Range(randomBounds.min.x, randomBounds.max.x),
                randomBounds.min.y,
                Random.Range(randomBounds.min.z, randomBounds.max.z));
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
    private void GetValidSpawnLocations()
    {
        indexOfValidSpawnPoints.Clear();
        Vector3 playerPosition = playerTransform.position; // cache this becasue I don't want to run it for every single loop.
        for (int i = 0; i < allBoundsList.Count; i++)
        {
            // -- Conditional reasons to NOT spawn something.
            if (
                AreWeTooCloseToSpawnPoint(allBoundsList[i],playerPosition))
            {
                continue;
            }
            else
            {
                indexOfValidSpawnPoints.Add(i);
            }
        }
    }
    /// <summary>
    /// Determines if the player is too close to the spawn. If so, invalidates the spawn point.
    /// </summary>
    /// <param name="bounds">This is the bounds to check against the distance against.</param>
    /// <param name="playerPosition">Object we are comparing the distance to--typically the player??</param>
    /// <returns>Returns true if the bounds is too close to the object. Reutrns false if the object is NOT too close.</returns>
    private bool AreWeTooCloseToSpawnPoint(Bounds bounds, Vector3 playerPosition)
    {        
        float distance = Vector3.Distance(bounds.ClosestPoint(playerPosition), playerPosition);
        if(distance > closestDistanceUntilWeStopSpawning)
        {
            return false;
        }
        else
        {
            return true;
        }            
    }


    /// <summary>
    /// Weak method and very basic. Maybe we will add more possibilities.
    /// </summary>
    IEnumerator Spawn() // Temp Testing, Remove later.
    {
        GatherBoundsForList();
        for (int i = 0; i < TotalEnemiesToSpawn; i++)
        {
            // Wait until there is room to spawn
            while (enemySpawnCount >= maxEnemies)
            {
                yield return null; // wait one frame, check again
            }

            yield return new WaitForSeconds(Random.Range(.01f, .01f));

            GameObject winner = PoolManager.Instance.Rent(prefab[Random.Range(0, prefab.Length)]);
            winner.transform.position = GetRandomSpawnLocation();
            if(indexOfValidSpawnPoints.Count >0)
            {
                enemySpawnCount++;
            }
            
        }
    }
}
