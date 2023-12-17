using Dobeil;
using NetworkManagerModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class OnlineServer : MonoSingleton<OnlineServer>
{
    [SerializeField] private TCPServer TCPServer;
    [SerializeField] private UDPServer UDPServer;

    public Action<string> onTcpServerReciveMessage;
    public Action<string> onUdpServerReciveMessage;

    public LobbyManager currentLobbyTofill;
    public LobbyManager lobbyManagerPrefab;
    private Dictionary<string, LobbyManager> lobbyHash;
    private Queue<OnlineServerMessageClass> messageQueue;
    private int lobbyCount = 0;
    private int clientCount = 0;


    private void Start()
    {
        messageQueue = new Queue<OnlineServerMessageClass>();
        lobbyHash = new Dictionary<string, LobbyManager>();
        Application.targetFrameRate = 60;
        TCPServer.StartServer(5555);
        UDPServer.StartServer(1234);
    }
    public void SendMessageToClient(int clientId, string message, SendMessageProtocol protocol)
    {
        switch (protocol)
        {
            case SendMessageProtocol.UDP:
                UDPServer.SendMessageToServere(clientId, message);
                break;
            case SendMessageProtocol.TCP:
                TCPServer.SendMessageToServer(clientId, message);
                break;
        }
    }
    public void ReciveMessageHandler(string message, int clientId, Action<int> callBack = null)
    {
        messageQueue.Enqueue(new OnlineServerMessageClass(message, clientId, callBack));
    }

    private void CheckMessageInQueue(OnlineServerMessageClass _messageClass)
    {
        string[] messageArray = _messageClass.message.Split(':');
        int indexInMessage = 0;
        if (int.TryParse(messageArray[indexInMessage], out int messageId))
        {
            if (Enum.IsDefined(typeof(ClientToServerId), messageId))
            {
                indexInMessage++;
                switch ((ClientToServerId)messageId)
                {
                    case ClientToServerId.Validation:
                        ValidationMessageHandler(messageArray, indexInMessage, _messageClass.callBack);
                        break;
                    case ClientToServerId.SelectedHero:
                        SelectedHeroMessageHandler(messageArray, indexInMessage, _messageClass.clientId);
                        break;
                    case ClientToServerId.CharacterInput:
                        UserInputMessageHandler(messageArray, indexInMessage, _messageClass.clientId);
                        break;
                    case ClientToServerId.Ping:
                        PingMessageHandler(messageArray, indexInMessage, _messageClass.clientId);
                        break;
                }
            }
        }
    }
    #region Recive MessageHandler Functions 
    private void ValidationMessageHandler(string[] message, int indexInMessage, Action<int> callBack = null)
    {
        string userKey = message[indexInMessage];
        //TODO Check User Key
        indexInMessage++;
        int clientId = ClientIdGenerator();
        callBack?.Invoke(clientId);
        if (currentLobbyTofill == null || currentLobbyTofill.isFull)
        {
            currentLobbyTofill = Instantiate(lobbyManagerPrefab, transform);
            string lobbyKey = keyGenerator();
            currentLobbyTofill.lobbyKey = lobbyKey;
            lobbyHash.Add(lobbyKey, currentLobbyTofill);
        }
        currentLobbyTofill.AddNewPlayer(clientId, (currentLobbyTofill.usersInThisLobby.Count % 2) == 0);
        string messageToClient = $"{(int)ServerToClientId.validation}:{1}:{currentLobbyTofill.lobbyKey}";
        SendMessageToClient(clientId, messageToClient, SendMessageProtocol.TCP);
    }
    private string keyGenerator()
    {
        return $"lobby{lobbyCount++}";
    }
    private int ClientIdGenerator()
    {
        return clientCount++;
    }


    private void SelectedHeroMessageHandler(string[] message, int indexInMessage, int clientId)
    {
        string lobbyId = message[indexInMessage];
        indexInMessage++;
        if (Instance.lobbyHash.ContainsKey(lobbyId))
        {
            lobbyHash[lobbyId].OnUserSelectHero(clientId, message, indexInMessage);
        }
    }
    private void UserInputMessageHandler(string[] message, int indexInMessage, int clientId)
    {
        string lobbyId = message[indexInMessage];
        indexInMessage++;
        if (Instance.lobbyHash.ContainsKey(lobbyId))
        {
            float inputX = float.Parse(message[indexInMessage]);
            indexInMessage++;
            float inputY = float.Parse(message[indexInMessage]);
            indexInMessage++;
            Vector2 userInput = new Vector2(inputX, inputY);
            Instance.lobbyHash[lobbyId].UserInputManager(clientId, userInput);
        }
    }

    private void PingMessageHandler(string[] message, int indexInMessage, int clientId)
    {
        string pongMessage = $"{ServerToClientId.Pong}:{message[indexInMessage]}";
        SendMessageToClient(clientId, pongMessage, SendMessageProtocol.UDP);
    }

    #endregion

    #region Send Message Handler Functions
    public void BroadCastMessageInLobby(string message, string lobbyKey, SendMessageProtocol protocol)
    {
        LobbyManager currentLobby;
        if (lobbyHash.TryGetValue(lobbyKey, out currentLobby))
        {
            foreach (var userId in currentLobby.usersInThisLobby.Keys)
                SendMessageToClient(userId, message, protocol);
        }
    }
    #endregion

    private void FixedUpdate()
    {
        if (messageQueue.Count > 0)
        {
            CheckMessageInQueue(messageQueue.Dequeue());
            Debug.Log(messageQueue.Count);
        }
    }
}
