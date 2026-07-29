using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Rendering;

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
        ChangeToMenu(audioMenu);
    }
    public void GraphicsButtonWasPressed()
    {
        ChangeToMenu(graphicsMenu);
    }

    public void BackButtonWasPressed()
    {        
        settingsMenu.SetActive(false);
        audioMenu.SetActive(false);
        escMenu.SetActive(true);
    }

    // -- Supplemental Functions -- //
    private void ChangeToMenu(GameObject menuToTurnOn)
    {
        if(menuToTurnOn == currentAcitvatedMenu) { return; }
        if(currentAcitvatedMenu != null) { currentAcitvatedMenu.SetActive(false); }        
        menuToTurnOn.SetActive(true);
        currentAcitvatedMenu = menuToTurnOn;
    }
}
