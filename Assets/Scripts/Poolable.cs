using System.Collections;
using UnityEngine;

public class Poolable : MonoBehaviour
{    
    public int PoolIndex { get; set; }
    public PoolManager.PoolType typeOfPool;

    // -- temp. delete everything below later -- //
    private void OnEnable()
    {
        StartCoroutine(Return());
    }
    IEnumerator Return()
    {
        yield return new WaitForSeconds(Random.Range(.5f, 1.5f));
        PoolManager.Instance.PutBack(this.gameObject);
    }
}
