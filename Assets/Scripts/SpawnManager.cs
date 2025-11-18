using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    /// <summary>
    /// To Do
    /// 1. Multi Spawn Points
    /// </summary>
    public static SpawnManager instance;

    [SerializeField] GameObject[] prefab; // Temp Testing, Remove later.
    [SerializeField] GameObject[] spawnPoints;

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
    IEnumerator Spawn() // Temp Testing, Remove later.
    {
        GetRandomSpawnLocation(spawnPoints[0]);
        for (int i = 0; i < 900; i++)
        {            
            yield return new WaitForSeconds(Random.Range(.05f, .1f));
            GameObject winner = PoolManager.Instance.Rent(prefab[Random.Range(0, prefab.Length)]);
            winner.transform.position = new Vector3(Random.Range(x1, x2), y, Random.Range(z1, z2));
        }
    }
    /// <summary>
    /// Calculates the bounds of the box collider to use for spawn location
    /// </summary>
    /// <remarks>
    /// Ultimately we are getting the X range, Y Value, and Z range. We use this box to determine potential spawn locations.
    /// </remarks>
    /// <param name="spawnPointCollider"></param>
    private void GetRandomSpawnLocation (GameObject spawnPointCollider)
    {
        if(spawnPointCollider.TryGetComponent<BoxCollider>(out var box))
        {
            x1 = box.bounds.max.x;
            x2 = box.bounds.min.x;
            y = box.bounds.min.y;
            z1 = box.bounds.max.z;
            z2 = box.bounds.min.z;
        }
        else
        {
            Debug.LogError($"[SpawnManager] No Box Collider on {spawnPointCollider.name}");
        }
    }
}
