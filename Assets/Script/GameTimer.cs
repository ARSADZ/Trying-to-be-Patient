using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public float gameTime = 600f;
    public Button timerButton;
    public TMP_Text timerText;
    public TMP_Text alertText;

    private UserManager userManager;
    private bool alertShown = false;

    private void Start()
    {
        userManager = FindObjectOfType<UserManager>();
        timerButton.onClick.AddListener(AddTime);
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        while (gameTime > 0)
        {
            yield return new WaitForSeconds(1f);
            gameTime -= 1f;

            int minutes = Mathf.FloorToInt(gameTime / 60);
            int seconds = Mathf.FloorToInt(gameTime % 60);
            timerText.text = $"{minutes:D2}:{seconds:D2}";

            if (gameTime <= 10 && !alertShown)
            {
                alertShown = true;
                StartCoroutine(ShowCountdownAlert());
            }
        }

        ForceQuitGame();
    }
    private IEnumerator ShowCountdownAlert()
    {
        for (int i = 10; i > 0; i--)
        {
            alertText.text = $"Game akan tertutup dalam {i} detik!";
            yield return new WaitForSeconds(1f);
        }
    }
    public void AddTime()
    {
        if (userManager.currentUser != null)
        {
            if (userManager.currentUser.xp >= 1)
            {
                userManager.currentUser.xp -= 1;
                gameTime += 300f;
                userManager.UpdateUI();
                alertShown = false;
                GameSaveManager.SaveGame(userManager.currentUser, 1);
                StartCoroutine(userManager.ShowAlert("Waktu bertambah 5 menit!"));
            }
            else
            {
                StartCoroutine(userManager.ShowAlert("XP tidak cukup!"));
            }
        }
    }
    void ForceQuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                        Application.Quit();
        #endif
    }
}
