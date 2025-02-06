using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public CanvasGroup mainPanel, optionPanel, accountPanel;
    public AudioSource musicSource;

    void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        SetActivePanel(mainPanel);
    }

    public void ShowOptions()
    {
        SetActivePanel(optionPanel);
    }

    public void ShowAccountManager()
    {
        SetActivePanel(accountPanel);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    private void SetActivePanel(CanvasGroup panel)
    {
        mainPanel.alpha = 0;
        mainPanel.interactable = false;
        mainPanel.blocksRaycasts = false;

        optionPanel.alpha = 0;
        optionPanel.interactable = false;
        optionPanel.blocksRaycasts = false;

        accountPanel.alpha = 0;
        accountPanel.interactable = false;
        accountPanel.blocksRaycasts = false;

        panel.alpha = 1;
        panel.interactable = true;
        panel.blocksRaycasts = true;
    }
}
