using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Enemy : MonoBehaviour
{

    /// <summary>
    /// 
    /// Left over from 6.30
    /// time to get the Min / Max / average amount of the time between spawns.
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// </summary>
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
                SceneDirector_SpawnManager.Instance.spawnTotalGap += TakeTime(SceneDirector_SpawnManager.Instance.watchSpawn);
                SceneDirector_SpawnManager.Instance.enemySpawnQTY++;
                break;
            case "EnemySpawnDual(Clone)":
                SceneDirector_SpawnManager.Instance.spawnDualTotalGap += TakeTime(SceneDirector_SpawnManager.Instance.watchSpawnDual);
                SceneDirector_SpawnManager.Instance.enemySpawnDualQTY++;
                break;
            case "EnemyDuration(Clone)":
                SceneDirector_SpawnManager.Instance.spawnDurationTotalGap += TakeTime(SceneDirector_SpawnManager.Instance.watchDuration);
                SceneDirector_SpawnManager.Instance.enemyDurationQTY++;
                break;
            case "EnemyDurationDual(Clone)":
                SceneDirector_SpawnManager.Instance.spawnDurationDualTotalGap += TakeTime(SceneDirector_SpawnManager.Instance.watchDurationDual);
                SceneDirector_SpawnManager.Instance.enemyDurationDualQTY++;
                break;
            default:
                Debug.LogWarning("Didn't find a suitable name. That's bad");
                break;
        }
    }
    private double TakeTime(Stopwatch watch)
    {
        double gap = 0;
        if (watch.IsRunning)
        {
            gap += watch.Elapsed.TotalSeconds;
        }
        watch.Restart();
        return gap;
    }
}
