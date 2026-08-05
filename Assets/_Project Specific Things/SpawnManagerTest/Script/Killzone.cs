using UnityEngine;

public class Killzone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PoolManager.Instance.PutBack(other.gameObject);
    }
}
