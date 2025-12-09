using System;
using UnityEngine;

public class SpawnKillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PoolManager.Instance.PutBack(other.gameObject);        
    }
}
