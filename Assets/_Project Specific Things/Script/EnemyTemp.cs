using UnityEngine;

public class EnemyTemp : MonoBehaviour
{
    private void OnDisable()
    {
        SpawnManager.instance.RegisterDespawn();
    }
}
