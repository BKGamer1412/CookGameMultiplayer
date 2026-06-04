using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance { get; private set; }

    public event EventHandler OnPlayerReadyChanged;
    private Dictionary<ulong, bool> playerReadyDictionary;

    private void Awake()
    {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);

        //only host/server dictionary stored
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                //this player is NOT ready
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            KitchenGameLobby.Instance.DeleteLobby();
            Loader.LoadNetwork(Loader.Scene.GameScene);
        }
    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        //this will set as all client dictionary
        playerReadyDictionary[clientId] = true;

        OnPlayerReadyChanged.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientId)
    {
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
    }
}
