using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using System;

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
    [System.NonSerialized] public Stopwatch watchSpawn = new();
    [System.NonSerialized] public Stopwatch watchSpawnDual = new();
    [System.NonSerialized] public Stopwatch watchDuration = new();
    [System.NonSerialized] public Stopwatch watchDurationDual = new();

    [Header("StopWatch Lists")]
    public List<double> listSpawn = new();
    public List<double> listSpawnDual = new();
    public List<double> listDuration = new();
    public List<double> listDurationDual = new();

    [Header("Total Time Between Spawns")]
    public double spawnTotalGap = 0;
    public double spawnDualTotalGap = 0;
    public double durationTotalGap = 0;
    public double durationDualTotalGap = 0;   

    private double calculatedAverageGapSpawn;
    private double calculatedAverageGapSpawnDual;
    private double calculatedAverageGapDuration;
    private double calculatedAverageGapDurationDual;


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
        DoMath();
        StartCoroutine(SpawnManager.Instance.SpawnBurst(enemeyBurst, spawnBurstQTY, spawnBurst));
        StartCoroutine(SpawnManager.Instance.SpawnBurst(enemeyBurstDual, spawnBurstDualQTY, spawnBurstDual));

        StartCoroutine(SpawnManager.Instance.Spawn(enemeySpawn, spawnQTY, spawn, spawnPerSecond));
        StartCoroutine(SpawnManager.Instance.Spawn(enemeySpawnDual, spawnDualQTY, spawnDual, spawnDualPerSecond));

        StartCoroutine(SpawnManager.Instance.SpawnByDuration(enemeyDuration, spawnDurationQTY, spawnDuration, spawnDurationEveryX));
        StartCoroutine(SpawnManager.Instance.SpawnByDuration(enemeyDurationDual, spawnDurationDualQTY, spawnDurationDual, spawnDurationDualEveryX));

        StartCoroutine(WaitThenGiveStats());
    }

    private void DoMath()
    {
        calculatedAverageGapSpawn = spawnTotalGap / listSpawn.Count;
        calculatedAverageGapSpawnDual = spawnDualTotalGap / listSpawnDual.Count;
        calculatedAverageGapDuration = durationTotalGap / listDuration.Count;
        calculatedAverageGapDurationDual = durationDualTotalGap / listDurationDual.Count;

        if (!double.IsNaN(calculatedAverageGapDuration))
        {
            float expectedIntervalSpawn = 1f / spawnPerSecond;
            float expectedIntervalSpawnDual = 1f / spawnDualPerSecond;

            if (isWithinTolerance(calculatedAverageGapSpawn, 3, expectedIntervalSpawn)) { Debug.Log($"Spawn Per Second {calculatedAverageGapSpawn} = Passed"); } else { Debug.Log($"Spawn Per Second {calculatedAverageGapSpawn} = Failed"); }
            if (isWithinTolerance(calculatedAverageGapSpawnDual, 3, expectedIntervalSpawnDual)) { Debug.Log($"Spawn Per Second Dual {calculatedAverageGapSpawnDual} = Passed"); } else { Debug.Log($"Spawn Per Second Dual {calculatedAverageGapSpawnDual} = Failed"); }
            if (isWithinTolerance(calculatedAverageGapDuration, 3, spawnDurationEveryX)) { Debug.Log($"Duration Every X {calculatedAverageGapDuration} = Passed"); } else { Debug.Log($"Duration Every X {calculatedAverageGapDuration} = Failed"); }
            if (isWithinTolerance(calculatedAverageGapDurationDual, 3, spawnDurationDualEveryX)) { Debug.Log($"Duration Every X Dual {calculatedAverageGapDurationDual} = Passed"); } else { Debug.Log($"Duration Every X Dual {calculatedAverageGapDurationDual} = Failed"); }
        }
    }

    private bool isWithinTolerance(double averageGap, float tolerancePercentage, float expectedValue)
    {
        if (Mathf.Abs((float)averageGap - expectedValue) <= tolerancePercentage/100*expectedValue){return true;} else { return false; }
    }

    private float FindDurationOfTest()
    {
        
        float temp1 = spawnQTY / spawnPerSecond;
        float temp2 = spawnDualQTY / spawnDualPerSecond;
        float temp3 = spawnDurationQTY * spawnDurationEveryX;
        float temp4 = spawnDurationDualQTY * spawnDurationDualEveryX;
        float maxvalue = Mathf.Max(temp1,temp2,temp3,temp4);
        return maxvalue;
    }

    IEnumerator WaitThenGiveStats()
    {
        Debug.Log($"waiting for {FindDurationOfTest() + 2} seconds");
        yield return new WaitForSeconds(FindDurationOfTest() + 2);
        DoMath();
    }
}
