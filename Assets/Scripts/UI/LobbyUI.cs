using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] Button mainMenuButton;
    [SerializeField] Button createLobbyButton;
    [SerializeField] Button quickJoinButton;
    [SerializeField] Button codeJoinButton;
    [SerializeField] LobbyCreateUI lobbyCreateUI;
    [SerializeField] TMP_InputField codeInputField;
    [SerializeField] TMP_InputField playerNameInputField;
    [SerializeField] Transform lobbyContainer;
    [SerializeField] Transform lobbyTemplate;
    [SerializeField] Button refreshListButton;
    private float refreshCooldown;
    private float refreshCooldownMax = 5f;
    private bool canRefreshList = true;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.LeaveLobby();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        createLobbyButton.onClick.AddListener(() =>
        {
            lobbyCreateUI.Show();
        });
        quickJoinButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.QuickJoin();
        });
        codeJoinButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.JoinWithCode(codeInputField.text);
        });

        refreshListButton.onClick.AddListener(() =>
        {
            if (canRefreshList)
            {
                canRefreshList = false;
                refreshCooldown = refreshCooldownMax;
                //call to query the lobby list
                KitchenGameLobby.Instance.ListLobbies();
            }
        });

        lobbyTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        playerNameInputField.text = KitchenGameMultiplayer.Instance.GetPlayerName();
        playerNameInputField.onEndEdit.AddListener((string newText) =>
        {
            KitchenGameMultiplayer.Instance.SetPlayerName(newText);
        });

        KitchenGameLobby.Instance.OnLobbyListChanged += KitchenGameLobby_OnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());

    }

    private void KitchenGameLobby_OnLobbyListChanged(object sender, KitchenGameLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void Update()
    {
        refreshCooldown -= Time.deltaTime;
        if (refreshCooldown < 0f)
        {
            canRefreshList = true;
        }
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in lobbyContainer)
        {
            if (child == lobbyTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
        }
    }

    private void OnDestroy()
    {
        KitchenGameLobby.Instance.OnLobbyListChanged -= KitchenGameLobby_OnLobbyListChanged;
    }

}
