using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Poolable;
using static PoolManager;

public class PoolManager : MonoBehaviour
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
    /// - if no stack / list / transform exists for a pool type, create them on the fly.???? Yes. Dictionary <poolType>, List<GameObject>} (or stack)
    /// - continue on with all the main methods.
    /// - Move PoolType to poolable instead of having the methods require it.
    /// </summary>


    public static PoolManager Instance { get; private set; }

    [SerializeField] GameObject prefab; // Temp Testing, Remove later.
    private Vector3 postion = new Vector3(0, 0, 0); // Temp Testing, Remove later.

    // -- Transform References -- //
    [SerializeField] private Transform enemyPoolTransform;
    [SerializeField] private Transform miscPoolTransform;
    [SerializeField] private Transform sfxPoolTransform;
    [SerializeField] private Transform uiPoolTransform;
    [SerializeField] private Transform vfxPoolTransform;
    private Transform currentParentTransform; // recycled variable so we can set the parent transform to whatever pool it needs to be.

    // -- Lists -- //
    private List<GameObject> enemyList = new();
    private List<GameObject> miscList = new();
    private List<GameObject> sfxList = new();
    private List<GameObject> uiList = new();
    private List<GameObject> vfxList = new();
    private List<GameObject> currentList; // recycled variable for current list to put objects in.

    // -- Stacks -- //
    private Stack<int> enemyIndexStack = new();
    private Stack<int> miscIndexStack = new();
    private Stack<int> sfxIndexStack = new();
    private Stack<int> uiIndexStack = new();
    private Stack<int> vfxIndexStack = new();
    private Stack<int> currentIndexStack; // recycled variable for current stack to put indexes in.


    // -- Specialty Methods -- //
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("[PoolManager] Multiple instances were created. Destroying duplicate instance.");
            Destroy(gameObject);
        }

    }
    void Start()
    {
        StartCoroutine(Spawn()); // delete me later
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
        GameObject genericObject = Instantiate(prefab);

        // -- Actual work on the object -- //

        if (genericObject.TryGetComponent<Poolable>(out var poolable))
        {
            PrepareObjectForPooling(poolable.typeOfPool);
            genericObject.SetActive(activate);
            genericObject.transform.SetParent(currentParentTransform);
            currentList.Add(genericObject);
            int index = currentList.Count - 1;
            if (!activate) currentIndexStack.Push(index); // meaning it's disabled then push index becaue it's available.
            poolable.PoolIndex = index;

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

        if (genericObject.TryGetComponent<Poolable>(out var poolable))
        {
            PrepareObjectForPooling(poolable.typeOfPool);
            currentIndexStack.Push(poolable.PoolIndex);
        }
        else
        {
            Debug.LogError($"Prefab missing Poolable component for {genericObject.name}");
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
        if (prefab.TryGetComponent<Poolable>(out var poolable))
        {
            PrepareObjectForPooling(poolable.typeOfPool);
            if (currentIndexStack != null && currentIndexStack.Count > 0)
            {
                int index = currentIndexStack.Pop();
                GameObject genericObject = currentList[index];
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
            Debug.LogError($"{prefab.name} is missing poolable. Huh?!");
            return null;
        }
    }

    // -- Supplemental Methods -- //
    /// <summary>
    /// Sets the variables corresponding to the specified poolType.
    /// </summary>
    /// <remarks>If the poolType is missing then we just put everything in miscellaneous</remarks>
    /// <param name="poolType">Use the public enum and PoolManager will set things up accordingly.</param>
    private void PrepareObjectForPooling(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.Enemy:
                currentParentTransform = enemyPoolTransform;
                currentList = enemyList;
                currentIndexStack = enemyIndexStack;
                break;
            case PoolType.Misc:
                currentParentTransform = miscPoolTransform;
                currentList = miscList;
                currentIndexStack = miscIndexStack;
                break;
            case PoolType.SFX:
                currentParentTransform = sfxPoolTransform;
                currentList = sfxList;
                currentIndexStack = sfxIndexStack;
                break;
            case PoolType.UI:
                currentParentTransform = uiPoolTransform;
                currentList = uiList;
                currentIndexStack = uiIndexStack;
                break;
            case PoolType.VFX:
                currentParentTransform = vfxPoolTransform;
                currentList = vfxList;
                currentIndexStack = vfxIndexStack;
                break;
            default:
                Debug.LogWarning($"[PoolManager] Looks like {poolType} doesn't have a case in the switch. Placing in misc for now");
                currentParentTransform = miscPoolTransform;
                currentList = miscList;
                currentIndexStack = miscIndexStack;
                break;
        }
    }

    public enum PoolType
    {
        Enemy,
        Misc,
        SFX,
        UI,
        VFX,
        Winter
    }

    // -- TEMP. DELETE LATER -- //
    IEnumerator Spawn()
    {
        for (int i = 0; i < 900; i++)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(.1f, .7f));
            GameObject winner = Rent(prefab); // Temp Testing, Remove later.
        }
    }
}
