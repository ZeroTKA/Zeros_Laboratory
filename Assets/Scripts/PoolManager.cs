using UnityEngine;

public class PoolManager : MonoBehaviour
{    
    public static PoolManager Instance;

    [SerializeField] GameObject prefab; // Temp Testing, Remove later.

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
        Create();
        PutBack();
        Rent();
    }



    // -- Main Methods -- //
    /// <summary>
    /// Creates a new object and adds it to the pool.
    /// </summary>
    /// <remarks>Think of this like a quartermaster. The quartermaster (PoolManager) uses a special spell 
    /// to conjure new weapons (GameObjects) when inventory is low. Mostly used internally.
    /// </remarks>
    private void Create()
    {

    }
    /// <summary>
    /// Returns the object to the pool for reuse.
    /// </summary>
    /// <remarks>Think of this like a quartermaster. When you are done with your weapon (GameObject), you return it to the quartermaster (PoolManager).
    /// The quatermaster thanks you for returning it in good condition so it can be used again later.
    /// </remarks>
    public void PutBack() // Return object to the pool
    {

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
    public GameObject Rent() // Get object from the pool
    {
        return gameObject;
    }
}
