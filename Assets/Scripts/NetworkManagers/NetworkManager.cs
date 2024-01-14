using Riptide;
using Riptide.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkManagerModels;
using System;
using System.IO;
using Dobeil;

public class NetworkManager : SingletonBase<NetworkManager>
{
    public Server Server { get; private set; }
    Dictionary<string, LobbyManager> lobbyHash;

    public ushort port;
    public ushort maxCountOfUsers;
    
    public LobbyManager currentLobbyTofill;
    public LobbyManager lobbyManagerPrefab;

    private int lobbyCount = 0;
	private int countOfMessage = 0;
	private int countOfBytes = 0;

	void Start()
    {
        Application.runInBackground = true;
		Application.targetFrameRate = 60;
		lobbyHash = new Dictionary<string, LobbyManager>();
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, true);
        Server = new Server();
        Server.Start(port, maxCountOfUsers);
        Server.TimeoutTime = 10000;
        Server.ClientConnected += NewPlayerConnected;
        Server.ClientDisconnected += PlayerLeft;
    }
    void FixedUpdate()
    {
        Server.Update();
    }
    private void NewPlayerConnected(object sender, ServerConnectedEventArgs e)
	{
        Debug.LogError($"Connected Cliend Id IS : {e.Client.Id}");
        StartCoroutine(MessageCounter());
    }
    private void PlayerLeft(object sender, ServerDisconnectedEventArgs e)
    {
        //File.AppendAllText(Application.dataPath + "network.txt",
            //$"count of Message {countOfMessage} and count of Bytes {countOfBytes} \n");
    }
    public string keyGenerator()
	{
        return $"lobby{lobbyCount++}";
	}
    #region Send Messages Method
    public void SendMessageToCustomUser(Message message, ushort customUserId)
    {
        countOfMessage++;
        countOfBytes += message.BytesInUse;
        Server.Send(message, customUserId);
    }
    public void SendMessageToAllUsersInLobby(Message message, string lobbyKey)
	{
		if (lobbyHash.TryGetValue(lobbyKey, out LobbyManager lobby))
		{
			foreach (ushort userId in lobby.playersInLobby.Keys)
                SendMessageToCustomUser(message, userId);
            
		}
	}
	IEnumerator MessageCounter()
	{
		while (true)
		{
			yield return new WaitForSeconds(1);
			if (countOfMessage == 0 && countOfBytes == 0)
				break;
			Debug.Log(countOfMessage + "    " + countOfBytes);
			countOfMessage = 0;
			countOfBytes = 0;

		}
	}
	#endregion


	#region MessageHandler
	//********************
	[MessageHandler((ushort)ClientToServerId.Validation)]
    public static void ValidateConnectedUser(ushort fromClientId, Message message)
    {
        string userId = message.GetString();
        if (Instance.currentLobbyTofill == null || Instance.currentLobbyTofill.isFull)
        {
            Instance.currentLobbyTofill = Instantiate(Instance.lobbyManagerPrefab, Instance.transform);
            string lobbyKey = Instance.keyGenerator();
            Instance.currentLobbyTofill.lobbyKey = lobbyKey;
            Instance.lobbyHash.Add(lobbyKey, Instance.currentLobbyTofill);
        }
        Instance.currentLobbyTofill.AddNewPlayer(
            fromClientId, 
            userId, 
            (Teams)(Instance.currentLobbyTofill.playersInLobby.Count % 2));
        Message serverMessage = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.validation);
        serverMessage.AddString(Instance.currentLobbyTofill.lobbyKey);
        Instance.Server.Send(serverMessage, fromClientId);
    }

	//********************
	[MessageHandler((ushort)ClientToServerId.SelectedHero)]
    public static void OnUsersSelectedHero(ushort fromClientId, Message message)
    {
        string lobbyId = message.GetString();
        if (Instance.lobbyHash.ContainsKey(lobbyId))
        {
            Instance.lobbyHash[lobbyId].OnUserSelectHero(fromClientId, message);
        }
    }

	//********************
	[MessageHandler((ushort)ClientToServerId.LoadGame)]
	public static void OnUsersLoadedGameScene(ushort fromClientId, Message message)
	{
		string lobbyId = message.GetString();
        ushort loadStep = message.GetUShort();
		if (Instance.lobbyHash.TryGetValue(lobbyId, out LobbyManager lobby))
			lobby.OnUserLoadedGame((Dobeil.LoadGameSteps)loadStep);
	}

	//********************
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

	//********************
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
