using System.Collections;
using UnityEngine;

public class Poolable : MonoBehaviour
{    
    public int PoolIndex { get; set; }
    public PoolManager.PoolType typeOfPool;

    // -- temp. delete later -- //
    private void OnEnable()
    {
        StartCoroutine(Return());
    }
    IEnumerator Return()
    {
        yield return new WaitForSeconds(Random.Range(1f, 5f));
        PoolManager.Instance.PutBack(this.gameObject);
    }
}
