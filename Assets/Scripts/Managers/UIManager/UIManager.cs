using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    public UIState CurrentState { get; private set; }
    public enum UIState { Pause, Settings, Inventory, Dialogue, Gameplay, MainMenu }

    public UnityEvent pauseMenu;
    public UnityEvent settingsMenu;
    public UnityEvent gamePlay;

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
        if (CurrentState == newState)
        {
            Debug.LogError($"[UIManager] We are already in the currentState: {newState} but we are trying to change to it again.");
        }
#endif
        CurrentState = newState;
        HandleStateChange();
    }
    private void HandleStateChange()
    {
        switch (CurrentState)
        {
            case UIState.Pause:
                pauseMenu?.Invoke();
                if (Cursor.lockState != CursorLockMode.None) { Cursor.lockState = CursorLockMode.None; }
                break;
            case UIState.Inventory:
                break;
            case UIState.Settings:
                settingsMenu?.Invoke();
                break;
            case UIState.Dialogue:
                break;
            case UIState.Gameplay:
                gamePlay?.Invoke();
                Cursor.lockState = CursorLockMode.Locked;
                break;
            case UIState.MainMenu:
                break;
            default:
                Debug.Log($"[UIManager] Missing a Switch case for {CurrentState}");
                break;
        }            
    }
}
