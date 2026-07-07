using UnityEngine;

public class SceneDirector_PoolManager : MonoBehaviour
{
    [SerializeField] GameObject testOneObject;
    [SerializeField] GameObject testOneSpawnPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawningSoManyThings();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawningSoManyThings()
    {
        StartCoroutine(SpawnManager.Instance.Spawn(testOneObject, 10, testOneSpawnPoint, 1));
    }
}
