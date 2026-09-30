using UnityEngine;

public class Poolable : MonoBehaviour
{
    /// <summary>
    /// The point of this script is to contain information about the object the PoolManager can use. This goes on any object that is pooled.
    /// 
    /// To Use: Add this script to any prefab that will be pooled. Add more information for the PoolManager as needed.
    /// 
    /// PoolIndex: Specifically used to immediately grab an object that's ready to be used from the pool.
    /// Without it we would have to iterate through a list until we found one that's available.
    /// 
    /// SourcePrefab: The prefab this object was created from. PoolManager sets it in Create() and reads it in PutBack()
    /// to know which pool the object belongs to. Pools are keyed by prefab, so each prefab gets its own pool automatically.
    /// You never set this yourself.
    /// </summary>


    public int PoolIndex { get; set; } //used by PoolManager to track objects in pool
    public GameObject SourcePrefab { get; set; } // used by PoolManager so it knows where it's supposed to go.

}
