using System;
using UnityEngine;

public class SpawnKillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PoolManagerTest.Instance.PutBack(other.gameObject);        
    }
}
