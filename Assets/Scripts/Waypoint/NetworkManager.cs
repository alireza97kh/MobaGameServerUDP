using Riptide;
using Riptide.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkManagerModels;
using System;

public class NetworkManager : SingletonBase<NetworkManager>
{
    public Server Server { get; private set; }
    Dictionary<string, LobbyManager> lobbyHash;

    public ushort port;
    public ushort maxCountOfUsers;
    
    public LobbyManager currentLobbyTofill;
    public LobbyManager lobbyManagerPrefab;

	// Start is called before the first frame update
	void Start()
    {
		Application.targetFrameRate = 60;
		lobbyHash = new Dictionary<string, LobbyManager>();
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, true);
        Server = new Server();
        Server.Start(port, maxCountOfUsers);
        Server.ClientConnected += NewPlayerConnected;
        Server.ClientDisconnected += PlayerLeft;
    }
    void Update()
    {
        Server.Update();
    }
    private void NewPlayerConnected(object sender, ServerConnectedEventArgs e)
	{
        Debug.LogError($"Connected Cliend Id IS : {e.Client.Id}");
    }
    private void PlayerLeft(object sender, ServerDisconnectedEventArgs e)
    {
    }
	int count = 0;
    public string keyGenerator()
	{
        return $"lobby{count++}";
	}
    #region Send Messages Method
    public void SendMessageToCustomUser(Message message, ushort customUserId)
    {
        Server.Send(message, customUserId);
    }

    public void SendMessageToAllUsersInLobby(Message message, string lobbyKey)
	{
		if (lobbyHash.ContainsKey(lobbyKey))
		{
			foreach (ushort userId in lobbyHash[lobbyKey].usersInThisLobby.Keys)
                SendMessageToCustomUser(message, userId);
            
		}
	}
    #endregion


    #region MessageHandler
    [MessageHandler((ushort)ClientToServerId.Validation)]
    public static void ValidateConnectedUser(ushort fromClientId, Message message)
    {
        Debug.LogError("Validation");
        string userKey = message.GetString();
        if (Instance.currentLobbyTofill == null || Instance.currentLobbyTofill.isFull)
        {
            Instance.currentLobbyTofill = Instantiate(Instance.lobbyManagerPrefab, Instance.transform);
            string lobbyKey = Instance.keyGenerator();
            Instance.currentLobbyTofill.lobbyKey = lobbyKey;
            Instance.lobbyHash.Add(lobbyKey, Instance.currentLobbyTofill);
        }
        Instance.currentLobbyTofill.AddNewPlayer(fromClientId, (Instance.currentLobbyTofill.usersInThisLobby.Count % 2) == 0);
        Message serverMessage = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.validation);
        serverMessage.AddString(Instance.currentLobbyTofill.lobbyKey);
        Instance.Server.Send(serverMessage, fromClientId);
    }
    [MessageHandler((ushort)ClientToServerId.SelectedHero)]
    public static void OnUsersSelectedHero(ushort fromClientId, Message message)
    {
        string lobbyId = message.GetString();
        if (Instance.lobbyHash.ContainsKey(lobbyId))
        {
            Instance.lobbyHash[lobbyId].OnUserSelectHero(fromClientId, message);
        }
    }

    [MessageHandler((ushort)ClientToServerId.CharacterInput)]
    public static void OnUserInputMessageGet(ushort fromClientId, Message message)
	{
        string lobbyId = message.GetString();
		if (Instance.lobbyHash.ContainsKey(lobbyId))
		{
            Vector2 userInput = message.GetVector2();
            Instance.lobbyHash[lobbyId].UserInputManager(fromClientId, userInput);
		}
    }

    [MessageHandler((ushort)ClientToServerId.Ping)]
    public static void OnPingMessageGet(ushort fromClientId, Message message)
    {
        Message pongMessage = Message.Create(MessageSendMode.Unreliable, ServerToClientId.Pong);
        int miliSecond = message.GetInt();
        pongMessage.AddInt(miliSecond);
        Instance.SendMessageToCustomUser(pongMessage, fromClientId);
    }
    #endregion


}
