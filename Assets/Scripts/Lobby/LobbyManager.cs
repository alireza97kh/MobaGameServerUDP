using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkManagerModels;
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
    public DictionaryWithEvent<int, CharacterControllerClass> usersInThisLobby = new DictionaryWithEvent<int, CharacterControllerClass>();
	public PlayersTeamData team1Data;
	public PlayersTeamData team2Data;

	public List<CreepGenerator> creepGenerators = new List<CreepGenerator>();

    int numberOfSelectedHero = 0;
    void Awake()
	{
        Instance = this;
        usersInThisLobby.ItemAddeed += OnUsersIdValueChange;
        usersInThisLobby.ItemRemoved += OnUsersIdValueChange;
        //NetworkManager.Instance.Server.ClientDisconnected += OnClientDisconnected;
    }
	#region Users List Value Event
	public void OnUsersIdValueChange(int key)
	{
		if (usersInThisLobby.Count == maxCountOfUser)
		{
            isFull = true;
			OnlineServer.Instance.BroadCastMessageInLobby(
				Utility.EnCodeMessage(new List<string>() { ((int)ServerToClientId.LobbyIsReady).ToString(), lobbyKey }),
				lobbyKey,
				SendMessageProtocol.TCP);
        }
		else if (usersInThisLobby.Count > maxCountOfUser)
		{
            DestroyImmediate(this);
		}
	}
    public void OnUsersIdValueChange(int key, CharacterControllerClass value)
    {
        if (usersInThisLobby.Count == maxCountOfUser)
        {
            Debug.Log("user Id is full");
            isFull = true;
			OnlineServer.Instance.BroadCastMessageInLobby(
				Utility.EnCodeMessage(new List<string>() { ((int)ServerToClientId.LobbyIsReady).ToString(), lobbyKey }),
				lobbyKey,
				SendMessageProtocol.TCP);
        }
        else if (usersInThisLobby.Count > maxCountOfUser)
        {
            DestroyImmediate(this);
        }
    }

    public void OnUserSelectHero(int fromClientId, string[] msg, int index)
	{
        string userId = msg[index];
		index++;
        string heroId = msg[index];
        usersInThisLobby[fromClientId].OnSelectedHero(heroId, userId, fromClientId, this);
        numberOfSelectedHero++;
        Debug.Log($"{userId} Selected Hero {heroId}");
		List<string> userSelectHeroMessage = new()
        {
            ((int)ServerToClientId.HeroSelected).ToString(),
            heroId,
            userId
        };
        OnlineServer.Instance.BroadCastMessageInLobby(Utility.EnCodeMessage(userSelectHeroMessage), lobbyKey, SendMessageProtocol.TCP);
		if (numberOfSelectedHero == usersInThisLobby.Count)
		{
			List<string> lobbyStartGameMessage = new List<string>();
			lobbyStartGameMessage.Add(((int)ServerToClientId.AllHeroSelected).ToString());
			foreach (var item in usersInThisLobby)
			{
                item.Value.Init();
			}
			//StartGame();
            OnlineServer.Instance.BroadCastMessageInLobby(Utility.EnCodeMessage(lobbyStartGameMessage), lobbyKey, SendMessageProtocol.TCP);
		}
	}
	#endregion

	//void OnClientDisconnected(object sender, ServerDisconnectedEventArgs e)
	//{
	//	if (usersInThisLobby.ContainsKey(e.Client.Id))
	//	{
	//           Debug.Log("Disconnected Client is on this Lobby ");
	//           usersInThisLobby.Remove(e.Client.Id);
	//		if (usersInThisLobby.Count == 0)
	//		{
	//			Destroy(gameObject);
	//		}
	//	}
	//}
	//#endregion


	//private void StartGame()
	//{
	//	Message creepGeneratorMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.CreateCreepGenerator);
	//	creepGeneratorMessage.AddInt(creepGenerators.Count);
	//	int generatorId = 0;
	//	foreach (var item in creepGenerators)
	//	{
	//		creepGeneratorMessage.AddInt((int)item.generatorData.generatorTeam);
	//		creepGeneratorMessage.AddInt((int)item.generatorData.generatorLine);
	//		creepGeneratorMessage.AddVector3(item.transform.position);
	//		creepGeneratorMessage.AddInt(generatorId);
	//		item.Init(generatorId, lobbyKey);
	//		generatorId++;
	//	}
	//	NetworkManager.Instance.SendMessageToAllUsersInLobby(creepGeneratorMessage, lobbyKey);
	//}
	//#region public Method
	public void AddNewPlayer(int playerId, bool isTeam1)
	{
		CharacterControllerClass newUser = Instantiate(characterPrefab, isTeam1 ? team1Parent : team2Parent);
		newUser.characterId = playerId;
		newUser.isTeam1 = isTeam1;
		newUser.character.Warp(isTeam1 ? team1Data.teamPlayerSpawnPosition : team2Data.teamPlayerSpawnPosition);
		usersInThisLobby.Add(playerId, newUser);

	}

	public void UserInputManager(int playerId, Vector2 input)
	{
		if (usersInThisLobby.ContainsKey(playerId))
		{
			usersInThisLobby[playerId].PlayerInputController(input);
		}
	}
	//#endregion

	//#region Private Functions
	//int timer = 0;
	//private void FixedUpdate()
	//{
	//	timer++;
	//	if (timer > tick)
	//	{
	//		SendSyncMessage();
	//		timer = 0;
	//	}
	//}
	//private void SendSyncMessage()
	//{
	//	Message syncMessage = Message.Create(MessageSendMode.Unreliable, ServerToClientId.Sync);
	//	syncMessage.AddInt(maxCountOfUser);
	//	foreach (var item in usersInThisLobby)
	//	{
	//		syncMessage.AddVector3(item.Value.transform.position);
	//		syncMessage.AddString(item.Value.userId);
	//	}
	//	NetworkManager.Instance.SendMessageToAllUsersInLobby(syncMessage, lobbyKey);
	//}
	//#endregion
}
