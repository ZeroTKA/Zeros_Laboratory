using UnityEngine;

public class Enemy_PoolManager : MonoBehaviour
{
    void Update()
    {
        gameObject.transform.position += new Vector3(0, 0, 4) * Time.deltaTime;
    }
    private void OnDisable()
    {
        SpawnManager.Instance.UnregisterSpawn();
    }
}
