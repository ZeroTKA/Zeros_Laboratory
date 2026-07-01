using Unity.VisualScripting;
using UnityEngine;

public class SceneDirector_SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject enemeyPrefab;
    [SerializeField] GameObject spawnPoint;

    private void Start()
    {
        StartCoroutine(SpawnManager.Instance.SpawnBurst(enemeyPrefab, 3, spawnPoint));
    }
}
