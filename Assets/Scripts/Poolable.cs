using System.Collections;
using UnityEngine;

public class Poolable : MonoBehaviour
{    
    public int PoolIndex { get; set; }
    public PoolManager.PoolType typeOfPool;
    
    private void Update()
    {
        Move();
    }

    // -- Testing Purposes. Delete! -- //
    private void Move()
    {
        transform.position += new Vector3(-1f, 0f, 0f) * Time.deltaTime * 6f;
    }
}
