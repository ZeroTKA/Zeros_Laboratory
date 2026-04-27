using UnityEngine;

public class AmmoHandler : MonoBehaviour
{
    private int _ammoInClip;
    public int AmmoInClip => _ammoInClip;
    public void AShotWasFired()
    {        
        _ammoInClip -= 1;
        if(_ammoInClip < 0)
        {
            Debug.LogWarning("[AmmoHander] You can't have a negative number in a clip");
        }
    }
}
