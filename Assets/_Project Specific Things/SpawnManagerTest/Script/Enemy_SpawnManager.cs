using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Enemy_SpawnManager : MonoBehaviour
{

    string objectName;
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position += new Vector3(0, 0, 4) * Time.deltaTime;
    }
    private void OnEnable()
    {
        GatherOnEnableData();
    }
    private void OnDisable()
    {
        SpawnManager.Instance.UnregisterSpawn();
    }

    // -- Supplement Methods -- //
    private void GatherOnEnableData()
    {
        objectName = gameObject.name;
        switch (objectName)
        {
            case "EnemyBurst(Clone)":
                SceneDirector_SpawnManager.Instance.enemyBurstQTY++;
                break;
            case "EnemyBurstDual(Clone)":
                SceneDirector_SpawnManager.Instance.enemyBurstDualQTY++;
                break;
            case "EnemySpawn(Clone)":
                SceneDirector_SpawnManager.Instance.spawnTotalGap += TakeTime(SceneDirector_SpawnManager.Instance.watchSpawn, SceneDirector_SpawnManager.Instance.listSpawn);
                SceneDirector_SpawnManager.Instance.enemySpawnQTY++;
                break;
            case "EnemySpawnDual(Clone)":
                SceneDirector_SpawnManager.Instance.spawnDualTotalGap += TakeTime(SceneDirector_SpawnManager.Instance.watchSpawnDual, SceneDirector_SpawnManager.Instance.listSpawnDual);
                SceneDirector_SpawnManager.Instance.enemySpawnDualQTY++;
                break;
            case "EnemyDuration(Clone)":
                SceneDirector_SpawnManager.Instance.durationTotalGap += TakeTime(SceneDirector_SpawnManager.Instance.watchDuration, SceneDirector_SpawnManager.Instance.listDuration);
                SceneDirector_SpawnManager.Instance.enemyDurationQTY++;
                break;
            case "EnemyDurationDual(Clone)":
                SceneDirector_SpawnManager.Instance.durationDualTotalGap += TakeTime(SceneDirector_SpawnManager.Instance.watchDurationDual, SceneDirector_SpawnManager.Instance.listDurationDual);
                SceneDirector_SpawnManager.Instance.enemyDurationDualQTY++;
                break;
            default:
                Debug.LogWarning("Didn't find a suitable name. That's bad");
                break;
        }
    }
    private double TakeTime(Stopwatch watch,List<double> list)
    {
        double gap = 0;
        if (watch.IsRunning)
        {
            gap += watch.Elapsed.TotalSeconds;
            list.Add(watch.Elapsed.TotalSeconds);
        }
        watch.Restart();
        return gap;
    }
}
