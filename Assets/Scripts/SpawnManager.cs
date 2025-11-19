using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    /// <summary>
    /// To Do
    /// 1. Add conditional adding  of spawnpoints when getting them. Such as proximity? In camera view? these type of things.
    /// 2. Max enemies?
    /// </summary>
    public static SpawnManager instance;

    [SerializeField] GameObject[] prefab; // Temp Testing, Remove later.
    [SerializeField] GameObject[] spawnPoints;

    private List<Bounds> allBoundsList = new();

    private float x1;
    private float x2;
    private float y;
    private float z1;
    private float z2;

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
        StartCoroutine(Spawn());
    }

    // -- Main Method -- //


    // -- Supplemental Methods -- //
    /// <summary>
    /// Weak method and very basic. Maybe we will add more possibilities.
    /// </summary>
    /// <returns></returns>
    IEnumerator Spawn() // Temp Testing, Remove later.
    {
        GatherBoundsForList();
        for (int i = 0; i < 90000; i++)
        {            
            yield return new WaitForSeconds(Random.Range(.05f, .1f));
            GameObject winner = PoolManager.Instance.Rent(prefab[Random.Range(0, prefab.Length)]);
            winner.transform.position = GetRandomSpawnLocation();
        }
    }

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
        Bounds randomBounds = allBoundsList[Random.Range(0, allBoundsList.Count)];
        Vector3 spawnLocation = new Vector3(
            Random.Range(randomBounds.min.x, randomBounds.max.x),
            randomBounds.min.y,
            Random.Range(randomBounds.min.z, randomBounds.max.z));
        return spawnLocation;
    }
}
