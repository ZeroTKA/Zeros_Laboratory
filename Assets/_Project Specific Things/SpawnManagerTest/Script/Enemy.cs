using UnityEngine;

public class Enemy : MonoBehaviour
{
    

    string objectName;
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position += new Vector3(0, 0, 4) * Time.deltaTime;
    }
    private void OnEnable()
    {
        AddToQTY();
    }
    private void OnDisable()
    {
        SpawnManager.Instance.UnregisterSpawn();
    }

    // -- Supplement Methods -- //
    private void AddToQTY()
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
                SceneDirector_SpawnManager.Instance.enemySpawnQTY++;
                break;
            case "EnemySpawnDual(Clone)":
                SceneDirector_SpawnManager.Instance.enemySpawnDualQTY++;
                break;
            case "EnemyDuration(Clone)":
                SceneDirector_SpawnManager.Instance.enemyDurationQTY++;
                break;
            case "EnemyDurationDual(Clone)":
                SceneDirector_SpawnManager.Instance.enemyDurationDualQTY++;
                break;
            default:
                Debug.LogWarning("Didn't find a suitable name. That's bad");
                break;
        }
    }
    private void 
}
