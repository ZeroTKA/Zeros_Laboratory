using UnityEngine;

public class SceneDirector_UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIManager.Instance.ChangeState(UIManager.UIState.Gameplay);
    }

}
