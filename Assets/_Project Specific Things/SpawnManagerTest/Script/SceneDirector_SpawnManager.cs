using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;

public class SceneDirector_SpawnManager : MonoBehaviour
{
    public static SceneDirector_SpawnManager Instance { get; private set; }

    [Header("Spawn Quantity Configuration")]
    [SerializeField] int spawnBurstQTY = 100;
    [SerializeField] int spawnBurstDualQTY = 100;
    [SerializeField] int spawnQTY = 200;
    [SerializeField] int spawnDualQTY = 200;
    [SerializeField] int spawnDurationQTY = 200;
    [SerializeField] int spawnDurationDualQTY = 200;

    [Header("Spawn Timing Configuration")]
    [SerializeField] float spawnPerSecond = 15;
    [SerializeField] float spawnDualPerSecond = 15;
    [SerializeField] float spawnDurationEveryX = .06f;
    [SerializeField] float spawnDurationDualEveryX = .06f;

    [Header("Enemy Prefabs")]
    [SerializeField] GameObject enemeyBurst;
    [SerializeField] GameObject enemeyBurstDual;
    [SerializeField] GameObject enemeySpawn;
    [SerializeField] GameObject enemeySpawnDual;
    [SerializeField] GameObject enemeyDuration;
    [SerializeField] GameObject enemeyDurationDual;

    [Header("Spawn Points")]
    [SerializeField] GameObject spawnBurst;
    [SerializeField] GameObject[] spawnBurstDual;
    [SerializeField] GameObject spawn;
    [SerializeField] GameObject[] spawnDual;
    [SerializeField] GameObject spawnDuration;
    [SerializeField] GameObject[] spawnDurationDual;

    [Header("Stopwatches")]
    static Stopwatch watchSpawn;
    static Stopwatch watchSpawnDual;
    static Stopwatch watchDuration;
    static Stopwatch watchDurationDual;

    [Header("StopWatch Lists")]
    public List<double> listSpawn = new();
    public List<double> listSpawnDual = new();
    public List<double> listDuration = new();
    public List<double> listDurationDual = new();
   

    [Header("Actual QTY")]
    public int enemyBurstQTY = 0;
    public int enemyBurstDualQTY = 0;
    public int enemySpawnQTY = 0;
    public int enemySpawnDualQTY = 0;
    public int enemyDurationQTY = 0;
    public int enemyDurationDualQTY = 0;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("[SceneDirector_SpawnManager] Multiple instances were created. Destroying duplicate instance.");
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        StartCoroutine(SpawnManager.Instance.SpawnBurst(enemeyBurst, spawnBurstQTY, spawnBurst));
        StartCoroutine(SpawnManager.Instance.SpawnBurst(enemeyBurstDual, spawnBurstDualQTY, spawnBurstDual));

        StartCoroutine(SpawnManager.Instance.Spawn(enemeySpawn, spawnQTY, spawn, spawnPerSecond));
        StartCoroutine(SpawnManager.Instance.Spawn(enemeySpawnDual, spawnDualQTY, spawnDual, spawnDualPerSecond));

        StartCoroutine(SpawnManager.Instance.SpawnByDuration(enemeyDuration, spawnDurationQTY, spawnDuration, spawnDurationEveryX));
        StartCoroutine(SpawnManager.Instance.SpawnByDuration(enemeyDurationDual, spawnDurationDualQTY, spawnDurationDual, spawnDurationDualEveryX));
    }
}
