using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [Header("Escape Menu")]
    [SerializeField] GameObject escMenu;

    [SerializeField] Button quitButton;
    [SerializeField] Button resumeButton;
    [SerializeField] Button settingsButton;

    [Header("Settings Menu")]
    [SerializeField] GameObject settingsMenu;

    [SerializeField] Button audioButton;
    [SerializeField] Button backButton;
    [SerializeField] Button displayButton;
    [SerializeField] Button gameButton;
    [SerializeField] Button graphicsButton;
    [SerializeField] Button saveButton;
    [SerializeField] Button discardButton;
    [SerializeField] Button UIButton;
    [SerializeField] Button AccessibilityButton;

    [Header("Audio Menu")]
    [SerializeField] GameObject audioMenu;

    // -- Escape Menu Functions -- //
    public void EscapeMenuButtonPressed()
    {
        escMenu.SetActive(!escMenu.activeSelf);
        Cursor.lockState = escMenu.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
    }
    public void ResumeWasClicked()
    {
        EscapeMenuButtonPressed();
    }
    public void SettingsWasClicked()
    {
        settingsMenu.SetActive(true);
        escMenu.SetActive(false);
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
        audioMenu.SetActive(!audioMenu.activeSelf);
    }
}
