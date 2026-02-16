using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PoolManagerTest : MonoBehaviour
{
    /// <summary>
    /// High-level overview of how the PoolManager works:
    /// - Each pool type has its own parent transform for hierarchy organization.
    /// - Each pool maintains a list of objects and a stack of available indices.
    /// - When an object is requested, we pop a pre-cached index from the stack for fast retrieval,
    ///   avoiding costly iteration to find an available object.
    /// - Each pooled object requires a 'Poolable' script, which stores data about the object for the sake of the Pool Manager. 
    ///   Every object will know about itself and be able to communicate necessary data to the PoolManager when PutBack / Rented
    ///   
    /// </summary>

    /// <summary>
    /// Instructions:
    /// 1. Create your enums for PoolType.
    /// 2. Every object that will be pooled needs a 'Poolable' script attached to it.
    /// </summary>

    /// <summary>
    /// To Do List:
    /// 
    /// </summary>

    // -- TEST References. DELETE Between Lines -- //
    [SerializeField] private TextMeshProUGUI totalSpawnText;
    private int totalSpawnCount = 0;
    [SerializeField] private TextMeshProUGUI totalCreatedText;
    private int totalCreatedCount = 0;
    [SerializeField] private TextMeshProUGUI totalActiveText;
    private int totalReturnedCount = 0;
    [SerializeField] private TextMeshProUGUI averageCreateText;
    private System.Diagnostics.Stopwatch createStopwatch = new();
    [SerializeField] private TextMeshProUGUI averageReuseText;
    private System.Diagnostics.Stopwatch reuseStopwatch = new();
    private int totalReusedCount = 0;

    // -- TEST References. DELETE Between Lines -- //
    public static PoolManagerTest Instance { get; private set; }

    // -- Transform References -- //
    [SerializeField] private Transform masterPool;
    [SerializeField] private int maxPoolSize; // Logs warnings if a pool gets too big. This helps spot faulty logic with run-away pools.

    // -- Dictionary -- //
    private readonly Dictionary<PoolType, List<GameObject>> poolLists = new();
    private readonly Dictionary<PoolType, Stack<int>> poolStacks = new();
    private readonly Dictionary<PoolType, Transform> poolTransforms = new();

    // -- Specialty Methods -- //
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("[PoolManagerTest] Multiple instances were created. Destroying duplicate instance.");
            Destroy(gameObject);
        }
        foreach (PoolType type in System.Enum.GetValues(typeof(PoolType)))
        {
            poolLists[type] = new List<GameObject>();
            poolStacks[type] = new Stack<int>();

            // -- transform parenting -- //
            GameObject poolTransform = new(type.ToString());
            poolTransform.transform.SetParent(masterPool);
            poolTransforms[type] = poolTransform.transform;
        }


    }
    private void OnDestroy()
    {
        Instance = null;
    }

    // -- Main Methods -- //
    /// <summary>
    /// Creates a new object and adds it to the pool.
    /// </summary>
    /// <remarks>Think of this like a quartermaster. The quartermaster (PoolManager) uses a special spell 
    /// to conjure new weapons (GameObjects) when inventory is low. Mostly used internally.
    /// </remarks>
    /// <param name="prefab">This is the prefab you want to get from the pool.</param>
    /// <param name="poolType">Use the public enum and PoolManager will set things up accordingly.</param>
    private GameObject Create(GameObject prefab, bool activate = true)
    {

        // -- Prep work -- //
        createStopwatch.Start(); // DELETE STOP WATCH 
        GameObject genericObject = Instantiate(prefab);
        createStopwatch.Stop();  // DELETE STOP WATCH

        // -- DELETE BETWEEN LINES (TESTING PURPOSES ONLY) -- //
        totalCreatedCount++;
        totalSpawnCount++;
        UpdateUI();
        // -- DELETE BETWEEN LINES (TESTING PURPOSES ONLY) -- //

        // -- Actual work on the object -- //

        if (genericObject.TryGetComponent<PoolableTest>(out var poolable))
        {
            EnsurePoolExists(poolable.typeOfPool);
            genericObject.SetActive(activate);
            genericObject.transform.SetParent(poolTransforms[poolable.typeOfPool]);
            poolLists[poolable.typeOfPool].Add(genericObject);
            int index = poolLists[poolable.typeOfPool].Count - 1;
            if (!activate)
            {
                poolStacks[poolable.typeOfPool].Push(index);
            }
            poolable.PoolIndex = index;
#if UNITY_EDITOR // if there's a run away pool, make sure to log warnings like mad.
            if (poolLists[poolable.typeOfPool].Count > maxPoolSize)
            {
                Debug.LogWarning($"[PoolManager] Pool '{poolable.typeOfPool}' has {poolLists[poolable.typeOfPool].Count} objects. Possible leak?");
            }
#endif
        }
        else
        {
            Debug.LogError($"Prefab missing Poolable component at index {genericObject.name}");
        }
        return genericObject;

    }
    /// <summary>
    /// Returns the object to the pool for reuse.
    /// </summary>
    /// <remarks>Think of this like a quartermaster. When you are done with your weapon (GameObject), you return it to the quartermaster (PoolManager).
    /// The quatermaster thanks you for returning it in good condition so it can be used again later.
    /// </remarks>
    public void PutBack(GameObject genericObject)
    {
        // -- DELETE BETWEEN LINES (TESTING PURPOSES ONLY) -- //
        totalReturnedCount++;
        SpawnManagerTest.instance.UnregisterSpawn();
        UpdateUI();
        // -- DELETE BETWEEN LINES (TESTING PURPOSES ONLY) -- //


        // -- Error checking and skipping objects that are already inactive. We assume anything that's inactive is already in the pool.
        if (genericObject == null)
        {
            Debug.LogError("[Pool Manager] Something tried to return a null object to the pool.");
            return;
        }
        if (!genericObject.activeSelf)
        {
            return;
        }
        // -- Now we return the things to the correct places
        genericObject.SetActive(false);

        if (genericObject.TryGetComponent<PoolableTest>(out var poolable))
        {
            poolStacks[poolable.typeOfPool].Push(poolable.PoolIndex);
        }
        else
        {
            Debug.LogError($"[Pool Manager] Prefab missing Poolable component for {genericObject.name}");
        }
    }
    /// <summary>
    /// Gives an object from the pool to the script that's calling it.
    /// </summary>
    /// <remarks>
    /// Think of this like a quartermaster. You go to the quartermaster (PoolManager) and ask for a weapon (GameObject).
    /// You signed a paper saying you'll return it when you're done (PutBack()). Don't you dare lose it.
    /// </remarks>
    /// <example>
    /// Example usage for grabbing a bullet from the PoolManager:
    /// GameObject bullet = PoolManager.Instance.Rent();
    /// bullet.transform.position = firePoint.position;
    /// bullet.SetActive(true);
    /// </example>
    public GameObject Rent(GameObject prefab)
    {
        if (prefab.TryGetComponent<PoolableTest>(out var poolable))
        {
            // -- If we have something in the stack, use it. -- //
            if (poolStacks[poolable.typeOfPool].Count > 0)
            {
                int index = poolStacks[poolable.typeOfPool].Pop();
                GameObject genericObject = poolLists[poolable.typeOfPool][index];
                reuseStopwatch.Start(); // DELETE STOP WATCH
                genericObject.SetActive(true);
                reuseStopwatch.Stop(); // DELETE STOP WATCH
                // -- DELETE BETWEEN LINES (TESTING PURPOSES ONLY) -- //
                totalSpawnCount++;
                totalReusedCount++;
                UpdateUI();
                // -- DELETE BETWEEN LINES (TESTING PURPOSES ONLY) -- //
                return genericObject;
            }
            else
            {
                GameObject genericObject = Create(prefab);
                return genericObject;
            }
        }
        else
        {
            Debug.LogError($"[PoolManager] {prefab.name} is missing the poolable script.");
            return null;
        }
    }

    // -- Supplemental Methods -- //
    /// <summary>
    /// This destroys all the objects in the pool, clears the list, and clears the stack. 
    /// </summary>
    /// <param name="type">The type of pool we are clearing.</param>
    public void ClearPool(PoolType type)
    {
        foreach (var obj in poolLists[type])
        {
            if (obj != null) Destroy(obj);
        }
        poolLists[type].Clear();
        poolStacks[type].Clear();
    }
    /// <summary>
    /// ClearPools() big brother. It's a nuke to all pools.
    /// </summary>
    public void ClearAllPools()
    {
        foreach (PoolType type in System.Enum.GetValues(typeof(PoolType)))
        {
            ClearPool(type);
        }
    }
    /// <summary>
    /// During creation, figures out if the list / stack / transform exist for the PoolType. If not, create them.
    /// </summary>
    /// <remarks>
    /// Truthfully this is all organizational purposes--at least for now. Maybe in the future the PoolType will have something else attached to it.
    /// </remarks>
    /// <param name="type">Enum describing what pool this object belongs to.</param>
    private void EnsurePoolExists(PoolType type)
    {
        // Fallback for unknown or newly added pool types
        if (!poolLists.ContainsKey(type))
        {
            poolLists[type] = new List<GameObject>();
        }
        if (!poolStacks.ContainsKey(type))
        {
            poolStacks[type] = new Stack<int>();
        }
        if (!poolTransforms.ContainsKey(type))
        {
            // -- transform parenting -- //
            GameObject poolTransform = new(type.ToString());
            poolTransform.transform.SetParent(masterPool);
            poolTransforms[type] = poolTransform.transform;
        }
    }
    /// <summary>
    /// This is used to preload at your time of choosing instead of during gameplay.
    /// </summary>
    /// <param name="prefab">GameObject we want to prewarm</param>
    /// <param name="count">How many times are we insantiating the prefab?</param>
    public void Prewarm(GameObject prefab, int count)
    {
        if (count < 0)
        {
            Debug.LogWarning($"[PoolManager] Prewarm() called with count: {count}. Must be positive!");
        }
        for (int i = 0; i < count; i++)
        {
            Create(prefab, false);
        }
    }
    /// <summary>
    /// Returns all active objects of the specified pool type to the pool.
    /// </summary>
    /// <remarks>Use this method to ensure that all currently active objects in the specified pool are
    /// deactivated and made available for reuse. This can be useful for resetting the pool's state or preparing for a
    /// new operation. The method does not affect inactive objects.</remarks>
    /// <param name="type">The type of object pool whose active objects will be returned.</param>
    public void ReturnAllActive(PoolType type)
    {
        for (int i = 0; i < poolLists[type].Count; i++)
        {
            GameObject obj = poolLists[type][i];
            if (obj != null && obj.activeSelf)
            {
                PutBack(obj);
            }
        }
    }

    // -- Query Methods -- //
    public int GetTotalCount(PoolType type) => poolLists[type].Count;
    public int GetAvailableCount(PoolType type) => poolStacks[type].Count;
    public int GetActiveCount(PoolType type) => GetTotalCount(type) - GetAvailableCount(type);

    public enum PoolType
    {
        Enemy,
        Misc,
        SFX,
        UI,
        VFX,
        Winter
    }

    // -- TEST METHODS, DELETE.
    public void UpdateUI()
    {
        totalSpawnText.text = "Total Enemies Spawned: " + totalSpawnCount;
        totalActiveText.text = "Active Enemies: " + (totalSpawnCount - totalReturnedCount);
        totalCreatedText.text = "Total Enemies Created: " + totalCreatedCount;
        averageCreateText.text = "Avg Time to Create: " +
            Math.Round((double)createStopwatch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency * 1000.0 / totalCreatedCount, 5) +
            "ms";
        if (totalReturnedCount > 0)
        {
            averageReuseText.text = "Avg Time to Reuse: " +
                Math.Round((double)reuseStopwatch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency * 1000.0 / totalReusedCount, 5) +
                "ms";
        }
        else
        {
            averageReuseText.text = "Avg Time to Reuse: No Data";
        }

    }
}
