using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
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

    [Header("Audio Menu")]
    [SerializeField] GameObject audioMenu;

    // -- Escape Menu Functions -- //
    public void EscapeMenuButtonPressed()
    {
        escMenu.SetActive(!escMenu.activeSelf);
    }
    public void ResumeWasClicked()
    {

    }
    public void SettingsWasClicked()
    {

    }
    public void QiitWasClicked()
    {

    }
}
