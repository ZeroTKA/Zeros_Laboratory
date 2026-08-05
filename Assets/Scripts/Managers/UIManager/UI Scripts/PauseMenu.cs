using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    GameObject currentAcitveMenu;
    [Header("Escape Menu")]
    [SerializeField] GameObject escMenu;

    [Header("Settings Menu")]
    [SerializeField] GameObject settingsMenu;

    [Header("Audio Menu")]
    [SerializeField] GameObject audioMenu;

    [Header("Graphics Menu")]
    [SerializeField] GameObject graphicsMenu;

    // -- Escape Menu Methods -- //
    /// <summary>
    /// Either brings up or takes down Escape Menu.
    /// </summary>
    public void EscapeMenuButtonPressed()
    {
        escMenu.SetActive(!escMenu.activeSelf);
    }
    /// <summary>
    /// Does everything to bring us back to Gameplay.
    /// </summary>
    public void ResumeWasClicked()
    {
        EscapeMenuButtonPressed();
        UIManager.Instance.ChangeState(UIManager.UIState.Gameplay);
    }
    /// <summary>
    /// Does everything to bring up the Settings Menu.
    /// </summary>
    public void SettingsWasClicked()
    {
        settingsMenu.SetActive(true);
        escMenu.SetActive(false);
        UIManager.Instance.ChangeState(UIManager.UIState.Settings);
    }
    /// <summary>
    /// Stops literally everything.
    /// </summary>
    public void QuitWasClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif        
    }

    // -- Settings Menu Methods -- //
    /// <summary>
    /// Does all the things to show Audio Settings.
    /// </summary>
    public void AudioButtonWasPressed()
    {
        ChangeToMenu(audioMenu);
    }
    /// <summary>
    /// Does all the things to show Graphics Settings.
    /// </summary>
    public void GraphicsButtonWasPressed()
    {
        ChangeToMenu(graphicsMenu);
    }
    /// <summary>
    /// Does all the things to get us back to the Escape Menu.
    /// </summary>
    public void BackButtonWasPressed()
    {
        ChangeToMenu(escMenu);
        settingsMenu.SetActive(false);
        UIManager.Instance.ChangeState(UIManager.UIState.Pause);
    }

    // -- Supplemental Methods -- //
    /// <summary>
    /// Pass in whatever GameObject you wish to be active--it'll also turn off the current menu.
    /// </summary>
    /// <param name="menuToTurnOn">GameObject to be visible.</param>
    private void ChangeToMenu(GameObject menuToTurnOn)
    {
        if (menuToTurnOn == currentAcitveMenu) { return; }
        if (currentAcitveMenu != null) { currentAcitveMenu.SetActive(false); }
        menuToTurnOn.SetActive(true);
        currentAcitveMenu = menuToTurnOn;
    }
}
