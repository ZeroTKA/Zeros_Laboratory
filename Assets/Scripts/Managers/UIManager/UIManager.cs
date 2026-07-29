using UnityEngine;

public class UIManager : MonoBehaviour
{
    private UIState currentState;
    public enum UIState { Pause, Inventory, Dialogue, Gameplay }

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning("[UIManager] Multiple instances were created. Destroying duplicate instance.");
#endif
            Destroy(gameObject);
            return;
        }
    }

    public void ChangeState(UIState newState)
    {
#if UNITY_EDITOR
        if(currentState == newState)
        {
            Debug.LogError($"[UIManager] We are already in the currentState: {newState} but we are trying to change to it again.");
        }
#endif
        currentState = newState;
    }
}
