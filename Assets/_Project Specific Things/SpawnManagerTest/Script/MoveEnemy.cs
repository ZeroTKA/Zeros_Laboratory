using UnityEngine;

public class MoveEnemy : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position += new Vector3(0, 0, 4) * Time.deltaTime;
    }
    private void OnDisable()
    {
        SpawnManager.Instance.UnregisterSpawn();
    }
}
