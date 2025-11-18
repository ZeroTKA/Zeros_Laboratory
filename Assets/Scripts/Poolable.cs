using System.Collections;
using UnityEngine;

public class Poolable : MonoBehaviour
{    
    public int PoolIndex { get; set; }
    public PoolManager.PoolType typeOfPool;
    private void OnEnable()
    {
        StartCoroutine(Despawn());
    }
    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(Random.Range(3f, 4f));
        PoolManager.Instance.PutBack(gameObject);
    }
}
