using System.Collections.Generic;
using UnityEngine;


public class PoolManager : MonoBehaviour
{
    /// <summary>
    /// High-level overview of how the PoolManager works:
    /// - Each pool type has its own parent transform to keep the hierarchy organized.
    /// - Each pool maintains a list of objects and a stack of available indices.
    /// - When an object is requested, the PoolManager pops a pre-cached index from the stack,
    ///   allowing fast retrieval without iterating to find a free object.
    /// - Each pooled object must have a 'Poolable' component, which stores metadata used by the PoolManager.
    ///   This allows objects to provide information to the PoolManager when they are Rented() and PutBack().
    ///   
    /// - The PoolManager expects each object to handle its own reset logic when returned.
    ///   Resetting state is the responsibility of the object itself, not the PoolManager.
    /// - The PoolManager expects calling scripts to handle all positioning and movement.
    ///   Its only responsibility is to provide a valid pooled object.
    /// </summary>

    /// <summary>
    /// Instructions:
    /// 1. Add the Poolable component to any prefab you want to be pooled, and assign its PoolType in the inspector.
    /// 2. If you're introducing a new pool category, add the corresponding PoolType value to the PoolManager enum.
    /// 3. Assign masterPool in the PoolManager inspector to a dedicated GameObject in your scene for organization.
    /// 4. Call PoolManager.Instance.Rent(prefab) to retrieve an object from the pool (returned object is active).
    /// 5. Call PoolManager.Instance.PutBack(obj) to return the object to the pool when you're finished with it.
    /// 6. (Optional) Call PoolManager.Instance.Prewarm(prefab, count) at scene start to pre-populate pools.
    /// 7. (Optional) Use GetTotalCount/GetAvailableCount/GetActiveCount for debugging pool state.
    /// 8. (Optional) Call ClearPool(type) or ClearAllPools() when transitioning scenes or resetting game state.
    /// </summary>
    

    /// <summary>
    /// To Do List:
    /// 
    /// </summary>

    public static PoolManager Instance { get; private set; }

    // -- Enums -- //
    public enum PoolType
    {
        Enemy
    }

    // -- Transform References -- //
    [SerializeField] private Transform masterPool; // this is the parent object that all pools go under.
    [SerializeField] private int maxPoolSize; // Logs warnings if a pool gets too big. This helps spot faulty logic with run-away pools.

    // -- Dictionary -- //
    private readonly Dictionary<PoolType, List<GameObject>> poolLists = new(); // our list of all objects per pool.
    private readonly Dictionary<PoolType, Stack<int>> poolStacks = new(); // used to cache indices that are ready to pop()
    private readonly Dictionary<PoolType, Transform> poolTransforms = new(); // organizational purposes.

    // -- Specialty Methods -- //

    /// <summary>
    /// All initialization is done in Awake so that everything is ready before other scripts run.
    /// Other scripts typically use Start(), which executes after Awake(), preventing null reference issues.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("[PoolManager] Multiple instances were created. Destroying duplicate instance.");
            Destroy(gameObject);
            return;
        }
        if(masterPool == null)
        {
            Debug.LogError("[PoolManager] masterPool is not assigned in the inspector!");
            return;
        }

        // -- Initialize dictionaries for each PoolType -- //
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
    /// <param name="activate">If true, the object will be active upon creation. Default is true. Useful for preloading.</param>
    private GameObject Create(GameObject prefab, bool activate = true)
    {
        // -- Prep work -- //
        GameObject genericObject = Instantiate(prefab);


        // -- Actual work on the object -- //
        if (genericObject.TryGetComponent<Poolable>(out var poolable))
        {
            EnsurePoolExists(poolable.typeOfPool);
            genericObject.SetActive(activate);
            genericObject.transform.SetParent(poolTransforms[poolable.typeOfPool]);
            poolLists[poolable.typeOfPool].Add(genericObject);
            int index = poolLists[poolable.typeOfPool].Count - 1;
            if (!activate)
            {
                // if the object is inactive, make sure to add it to the stack to be used. 
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
            Debug.LogError($"[PoolManager] Prefab missing Poolable component. It's named {genericObject.name}");
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
        // -- Error checking and skipping objects that are already inactive. We assume anything that's inactive is already in the pool.
        if (genericObject == null)
        {
            Debug.LogError("[PoolManager] Something tried to return a null object to the pool.");
            return;
        }
        if (!genericObject.activeSelf)
        {
            Debug.LogWarning($"[PoolManager] Something tried to return an already inactive object to the pool. The object is called {genericObject.name}");
            return;
        }

        // -- Now we return the things to the correct places -- //
        genericObject.SetActive(false);

        if (genericObject.TryGetComponent<Poolable>(out var poolable))
        {
            poolStacks[poolable.typeOfPool].Push(poolable.PoolIndex);
        }
        else
        {
            Debug.LogError($"[PoolManager] Prefab missing Poolable component for {genericObject.name}");
        }
    }
    /// <summary>
    /// Retrieves an object from the pool and gives it to the script.
    /// </summary>
    /// <remarks>
    /// Think of this like a quartermaster. You go to the quartermaster (PoolManager) and ask for a weapon (GameObject).
    /// You signed a paper saying you'll PutBack() when you're done. Don't you dare lose it.
    /// </remarks>
    public GameObject Rent(GameObject prefab)
    {
        if (prefab.TryGetComponent<Poolable>(out var poolable))
        {
            // -- If we have something in the stack, use it. -- //
            if (poolStacks[poolable.typeOfPool].Count > 0)
            {
                int index = poolStacks[poolable.typeOfPool].Pop();
                GameObject genericObject = poolLists[poolable.typeOfPool][index];
                genericObject.SetActive(true);
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
        if(count < 0)
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
}
