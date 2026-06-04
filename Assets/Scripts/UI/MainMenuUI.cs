using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button MultiplayButton;
    [SerializeField] private Button SingleplayButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        MultiplayButton.onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.multiplayer = true;
            Loader.Load(Loader.Scene.LobbyScene);
        });

        SingleplayButton.onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.multiplayer = false;
            Loader.Load(Loader.Scene.LobbyScene);
        });

        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        Time.timeScale = 1f;
    }

}
