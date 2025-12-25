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
    ///   
    /// </summary>

    /// <summary>
    /// Instructions:
    /// 1. Add the Poolable component to any prefab you want to be pooled, and assign its PoolType in the inspector.
    /// 2. If you're introducing a new pool category, add the corresponding PoolType value to the PoolManager enum.
    /// 3. Assign masterPool in the PoolManager inspector to a dedicated GameObject in your scene for organization.
    /// 4. Call PoolManager.Instance.Rent(prefab) to retrieve an object from the pool.
    /// 5. Call PoolManager.Instance.PutBack(obj) to return the object to the pool when you're finished with it.
    /// </summary>

    /// <summary>
    /// To Do List:
    /// 
    /// </summary>

    public static PoolManager Instance { get; private set; }    

    // -- Transform References -- //
    [SerializeField] private Transform masterPool; // this is the parent object that all pools go under.

    // -- Dictionary -- //
    private readonly Dictionary<PoolType, List<GameObject>> poolLists = new();
    private readonly Dictionary<PoolType, Stack<int>> poolStacks = new();
    private readonly Dictionary<PoolType, Transform> poolTransforms = new();

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
    /// <param name="poolType">Use the public enum and PoolManager will set things up accordingly.</param>
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
        // -- We assume if the object is already inactive, it's already in the pool.
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

    ///////////////////////////// FINISH CODE REVIEW FOR THE BELOW ///////////////////////////////////////
    /// <summary>
    /// Retrieves an object from the pool and gives it to the script.
    /// </summary>
    /// <remarks>
    /// Think of this like a quartermaster. You go to the quartermaster (PoolManager) and ask for a weapon (GameObject).
    /// You signed a paper saying you'll PutBack() when you're done. Don't you dare lose it.
    /// </remarks>
    /// <example>
    /// Example usage for grabbing a bullet from the PoolManager:
    /// GameObject bullet = PoolManager.Instance.Rent();
    /// bullet.transform.position = firePoint.position;
    /// bullet.SetActive(true);
    /// </example>
    public GameObject Rent(GameObject prefab)
    {
        if (prefab.TryGetComponent<Poolable>(out var poolable))
        {
            if(poolStacks[poolable.typeOfPool].Count > 0)
            {
                int index = poolStacks[poolable.typeOfPool].Pop();
                GameObject genericObject = poolLists[poolable.typeOfPool][index];

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
            Debug.LogError($"{prefab.name} is missing poolable. Huh?!");
            return null;
        }
    }

    // -- Supplemental Methods -- //
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

    public enum PoolType
    {
        Enemy
    }
}
