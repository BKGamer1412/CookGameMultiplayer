using System;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] Button resumeButton;
    [SerializeField] Button mainMenuButton;
    [SerializeField] Button optionsButton;

    private void Awake()
    {
        resumeButton.onClick.AddListener(() =>
        {
            KitchenGameManager.Instance.TogglePauseGame();
        });
        mainMenuButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        optionsButton.onClick.AddListener(() =>
        {
            Hide();

            //store this show function that when we close options UI it show GamePauseUI back
            OptionsUI.Instance.Show(Show);
        });

    }
    private void Start()
    {
        KitchenGameManager.Instance.OnGamePause += KitchenGameManager_OnGamePause;
        KitchenGameManager.Instance.OnGameUnPause += KitchenGameManager_OnGameUnPause;
        Hide();
    }

    private void KitchenGameManager_OnGamePause(object sender, EventArgs e)
    {
        Show();
    }

    private void KitchenGameManager_OnGameUnPause(object sender, EventArgs e)
    {
        Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
        resumeButton.Select();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OptionsToPausedSelect()
    {
        resumeButton.Select();
    }
}
