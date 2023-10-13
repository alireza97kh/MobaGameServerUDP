using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkManagerModels;
using Riptide;
using Riptide.Utils;
using UnityEngine.SceneManagement;
using Dobeil;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance = null;
	public int tick = 10;
    public string lobbyKey;
    public bool isFull = false;
    public int maxCountOfUser = 1;
    public Transform team1Parent;
    public Transform team2Parent;
    public CharacterControllerClass characterPrefab;
    public DictionaryWithEvent<ushort, CharacterControllerClass> usersInThisLobby = new DictionaryWithEvent<ushort, CharacterControllerClass>();
	public PlayersTeamData team1Data;
	public PlayersTeamData team2Data;

	public List<CreepGenerator> creepGenerators = new List<CreepGenerator>();

    int numberOfSelectedHero = 0;
    void Awake()
	{
        Instance = this;
        usersInThisLobby.ItemAddeed += OnUsersIdValueChange;
        usersInThisLobby.ItemRemoved += OnUsersIdValueChange;
        NetworkManager.Instance.Server.ClientDisconnected += OnClientDisconnected;
    }
	#region Users List Value Event
	public void OnUsersIdValueChange(ushort key)
	{
		if (usersInThisLobby.Count == maxCountOfUser)
		{
            isFull = true;
            Message lobbyMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.LobbyIsReady);
            lobbyMessage.AddString(lobbyKey);
            NetworkManager.Instance.SendMessageToAllUsersInLobby(lobbyMessage, lobbyKey);
        }
		else if (usersInThisLobby.Count > maxCountOfUser)
		{
            DestroyImmediate(this);
		}
	}
    public void OnUsersIdValueChange(ushort key, CharacterControllerClass value)
    {
        if (usersInThisLobby.Count == maxCountOfUser)
        {
            Debug.Log("user Id is full");
            isFull = true;
            Message lobbyMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.LobbyIsReady);
            lobbyMessage.AddString(lobbyKey);
            NetworkManager.Instance.SendMessageToAllUsersInLobby(lobbyMessage, lobbyKey);
        }
        else if (usersInThisLobby.Count > maxCountOfUser)
        {
            DestroyImmediate(this);
        }
    }

    public void OnUserSelectHero(ushort fromClientId, Message msg)
	{
        string userId = msg.GetString();
        string heroId = msg.GetString();
        usersInThisLobby[fromClientId].OnSelectedHero(heroId, userId, fromClientId, this);
        numberOfSelectedHero++;
        Debug.Log($"{userId} Selected Hero {heroId}");
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.HeroSelected);
        message.AddString(heroId);
        message.AddString(userId);
        NetworkManager.Instance.SendMessageToAllUsersInLobby(message, lobbyKey);
		if (numberOfSelectedHero == usersInThisLobby.Count)
		{
            Message lobbyStartGameMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.AllHeroSelected);
			foreach (var item in usersInThisLobby)
			{
                item.Value.Init();
			}
			StartGame();
            NetworkManager.Instance.SendMessageToAllUsersInLobby(lobbyStartGameMessage, lobbyKey);
		}
	}
	#endregion

	#region Events
	void OnClientDisconnected(object sender, ServerDisconnectedEventArgs e)
	{
		if (usersInThisLobby.ContainsKey(e.Client.Id))
		{
            Debug.Log("Disconnected Client is on this Lobby ");
            usersInThisLobby.Remove(e.Client.Id);
			if (usersInThisLobby.Count == 0)
			{
				Destroy(gameObject);
			}
		}
	}
	#endregion


	private void StartGame()
	{
		Message creepGeneratorMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.CreateCreepGenerator);
		creepGeneratorMessage.AddInt(creepGenerators.Count);
		int generatorId = 0;
		foreach (var item in creepGenerators)
		{
			creepGeneratorMessage.AddInt((int)item.generatorData.generatorTeam);
			creepGeneratorMessage.AddInt((int)item.generatorData.generatorLine);
			creepGeneratorMessage.AddVector3(item.transform.position);
			creepGeneratorMessage.AddInt(generatorId);
			item.Init(generatorId, lobbyKey);
			generatorId++;
		}
		NetworkManager.Instance.SendMessageToAllUsersInLobby(creepGeneratorMessage, lobbyKey);
	}
	#region public Method
	public void AddNewPlayer(ushort playerId, bool isTeam1)
	{
        CharacterControllerClass newUser = Instantiate(characterPrefab, isTeam1 ? team1Parent : team2Parent);
        newUser.characterId = playerId;
		newUser.isTeam1 = isTeam1;
		newUser.character.Warp(isTeam1 ? team1Data.teamPlayerSpawnPosition : team2Data.teamPlayerSpawnPosition);
        usersInThisLobby.Add(playerId, newUser);

	}

	public void UserInputManager(ushort playerId, Vector2 input)
	{
		if (usersInThisLobby.ContainsKey(playerId))
		{
			usersInThisLobby[playerId].PlayerInputController(input);
		}
	}
	#endregion

	#region Private Functions
	int timer = 0;
	private void FixedUpdate()
	{
		timer++;
		if (timer > tick)
		{
			SendSyncMessage();
			timer = 0;
		}
	}
	private void SendSyncMessage()
	{
		Message syncMessage = Message.Create(MessageSendMode.Unreliable, ServerToClientId.Sync);
		syncMessage.AddInt(maxCountOfUser);
		foreach (var item in usersInThisLobby)
		{
			syncMessage.AddVector3(item.Value.transform.position);
			syncMessage.AddString(item.Value.userId);
		}
		NetworkManager.Instance.SendMessageToAllUsersInLobby(syncMessage, lobbyKey);
	}
	#endregion
}
