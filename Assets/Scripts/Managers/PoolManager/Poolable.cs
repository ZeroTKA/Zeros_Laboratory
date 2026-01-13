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
    /// PoolType: Used by the PoolManager to know which pool this object belongs to. The script will dynamically create pools in the heirarchy as needed.
    /// This is more for organizational purposes and doesn't have much to do with efficiency. If you want one giant pool, go for it, the game won't care.
    /// </summary>


    public int PoolIndex { get; set; } //used by PoolManager to track objects in pool
    public PoolManager.PoolType typeOfPool; //used by PoolManager so it knows where it's supposed to go.

}
