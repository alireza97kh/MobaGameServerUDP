using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkManagerModels;
using Riptide;
using Riptide.Utils;

public class LobbyManager : MonoBehaviour
{
    public string lobbyKey;
    public bool isFull = false;
    public ListWithChangeEvent<ushort> usersId = new ListWithChangeEvent<ushort>();
    public int maxCountOfUser = 1;

    void Awake()
	{
        usersId.ItemAdded += OnUsersIdValueChange;
        usersId.ItemRemoved += OnUsersIdValueChange;
        NetworkManager.Instance.Server.ClientDisconnected += OnClientDisconnected;
    }

    public void OnUsersIdValueChange(ushort id)
	{
		if (usersId.Count == maxCountOfUser)
		{
            Debug.Log("user Id is full");
            isFull = true;
            Message lobbyMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.LobbyIsReady);
            lobbyMessage.AddString($"{lobbyKey} is Ready");
            NetworkManager.Instance.SendMessageToAllUsersInLobby(lobbyMessage, lobbyKey);
		}
		else if (usersId.Count > maxCountOfUser)
		{
            DestroyImmediate(this);
		}
	}

    void OnClientDisconnected(object sender, ServerDisconnectedEventArgs e)
	{
        Debug.LogError($"OnClientDisconnected {e.Client.Id}");
		if (usersId.Contains(e.Client.Id))
		{
            Debug.Log("Disconnected Client is on this Lobby ");
            usersId.Remove(e.Client.Id);

		}
	}

    // Update is called once per frame
    void Update()
    {
    }
}
