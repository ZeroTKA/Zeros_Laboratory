using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    GameObject currentAcitvatedMenu;
    [Header("Escape Menu")]
    [SerializeField] GameObject escMenu;

    [Header("Settings Menu")]
    [SerializeField] GameObject settingsMenu;

    [Header("Audio Menu")]
    [SerializeField] GameObject audioMenu;

    [Header("Graphics Menu")]
    [SerializeField] GameObject graphicsMenu;

    // -- Escape Menu Functions -- //
    public void EscapeMenuButtonPressed()
    {
        escMenu.SetActive(!escMenu.activeSelf);
    }
    public void ResumeWasClicked()
    {
        EscapeMenuButtonPressed();
        UIManager.Instance.ChangeState(UIManager.UIState.Gameplay);
    }
    public void SettingsWasClicked()
    {
        settingsMenu.SetActive(true);
        escMenu.SetActive(false);
        UIManager.Instance.ChangeState(UIManager.UIState.Settings);
    }
    public void QuitWasClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif        
    }

    // -- Settings Menu Functions -- //

    public void AudioButtonWasPressed()
    {
        ChangeToMenu(audioMenu);
    }
    public void GraphicsButtonWasPressed()
    {
        ChangeToMenu(graphicsMenu);
    }

    public void BackButtonWasPressed()
    {
        ChangeToMenu(escMenu);
        settingsMenu.SetActive(false);
        UIManager.Instance.ChangeState(UIManager.UIState.Pause);
    }

    // -- Supplemental Functions -- //
    private void ChangeToMenu(GameObject menuToTurnOn)
    {
        if (menuToTurnOn == currentAcitvatedMenu) { return; }
        if (currentAcitvatedMenu != null) { currentAcitvatedMenu.SetActive(false); }
        menuToTurnOn.SetActive(true);
        currentAcitvatedMenu = menuToTurnOn;
    }
}
