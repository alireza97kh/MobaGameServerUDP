using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkManagerModels;
using Riptide;
using Riptide.Utils;
using UnityEngine.SceneManagement;
using Dobeil;
using Sirenix.OdinInspector;
using System.Runtime.InteropServices.ComTypes;

public class LobbyManager : MonoBehaviour
{
    public ushort maxCountOfUser = 1;
	public DictionaryWithEvent<ushort, UserInLobbyData> playersInLobby;
	public Dictionary<int, ObjectInGameBase> objectsInGame = new Dictionary<int, ObjectInGameBase>();
	[TabGroup("Scriptable Objects")] public PlayersTeamData team1Data;
	[TabGroup("Scriptable Objects")] public PlayersTeamData team2Data;

	[TabGroup("Creep")] public List<CreepGenerator> creepGenerators = new List<CreepGenerator>();
	[TabGroup("Tower")] public TowerGenerator towerGenerator;

	[TabGroup("Content")] public Transform team1Parent;
	[TabGroup("Content")] public Transform team2Parent;

	[ReadOnly] public string lobbyKey;
    public bool isFull = false;
	public bool gameStarted = false;

    private int numberOfSelectedHero = 0;
    private int numberOfLoadedGameScene = 0;
    private int numberOfCreatedAllHeros = 0;
    private int numberOfCreatedAllTowers = 0;
    void Awake()
	{
		playersInLobby = new DictionaryWithEvent<ushort, UserInLobbyData>();
		playersInLobby.ItemAdded += OnUsersIdValueChange;
		playersInLobby.ItemRemoved += OnUsersIdValueChange;
        NetworkManager.Instance.Server.ClientDisconnected += OnClientDisconnected;
    }
	#region Users List Value Event
	public void OnUsersIdValueChange(ushort key, UserInLobbyData user)
	{
		if (playersInLobby.Count == maxCountOfUser)
		{
            isFull = true;
            Message lobbyMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.LobbyIsReady);
            lobbyMessage.AddString(lobbyKey);
            NetworkManager.Instance.SendMessageToAllUsersInLobby(lobbyMessage, lobbyKey);
        }
		else if (playersInLobby.Count > maxCountOfUser)
		{
            DestroyImmediate(this);
		}
	}
    public void OnUsersIdValueChange(ushort key)
    {
        if (playersInLobby.Count == maxCountOfUser)
        {
            isFull = true;
            Message lobbyMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.LobbyIsReady);
            lobbyMessage.AddString(lobbyKey);
            NetworkManager.Instance.SendMessageToAllUsersInLobby(lobbyMessage, lobbyKey);
        }
        else if (playersInLobby.Count > maxCountOfUser)
        {
            DestroyImmediate(this);
        }
    }

    public void OnUserSelectHero(ushort fromClientId, Message msg)
	{
		if (playersInLobby.TryGetValue(fromClientId, out UserInLobbyData user))
		{
			ushort heroId = msg.GetUShort();
			HeroControllerBase newHero = Instantiate(
				HeroManager.Instance.GetHeroPrefab((HeroId)heroId), 
				user.player.playerTeam == Teams.Team1 ? team1Parent : team2Parent);
			newHero.heroId = heroId;
			newHero.team = user.player.playerTeam;
			newHero.agent.Warp(user.player.playerTeam == Teams.Team1 
				? team1Data.teamPlayerSpawnPosition 
				: team2Data.teamPlayerSpawnPosition);
			user.hero = newHero;
			user.hero.OnSelectedHero(heroId, fromClientId, this);
			numberOfSelectedHero++;

			Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.HeroSelected);
			message.AddUShort(heroId);
			message.AddString(user.player.userId);
			NetworkManager.Instance.SendMessageToAllUsersInLobby(message, lobbyKey);
			if (numberOfSelectedHero == playersInLobby.Count)
			{
				Message lobbyStartGameMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.AllHeroSelected);
				NetworkManager.Instance.SendMessageToAllUsersInLobby(lobbyStartGameMessage, lobbyKey);
			}
		}
	}

	public void OnUserLoadedGame(LoadGameSteps loadStep)
	{
		switch (loadStep)
		{
			case LoadGameSteps.LoadGameScene:
				numberOfLoadedGameScene++;
				if (numberOfLoadedGameScene == playersInLobby.Count)
				{
					foreach (var item in playersInLobby)
						item.Value.hero.Init(this, item.Key);
				}
				break;
			case LoadGameSteps.LoadPlayers:
				numberOfCreatedAllHeros++;
				if (numberOfCreatedAllHeros == Mathf.Pow(playersInLobby.Count, 2))
					towerGenerator.Init(this);
				break;
			case LoadGameSteps.LoadTowers:
				numberOfCreatedAllTowers++;
				if (numberOfCreatedAllTowers == playersInLobby.Count)
					StartCreepGenerators();
				break;
			case LoadGameSteps.LoadCreeps:
				gameStarted = true;
				Message startGameMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.StartGame);
				NetworkManager.Instance.SendMessageToAllUsersInLobby(startGameMessage, lobbyKey);
				break;
			default:
				break;
		}
		
	}
	#endregion

	#region Events
	void OnClientDisconnected(object sender, ServerDisconnectedEventArgs e)
	{
		if (playersInLobby.ContainsKey(e.Client.Id))
		{
            Debug.Log("Disconnected Client is on this Lobby ");
            playersInLobby.Remove(e.Client.Id);
			if (playersInLobby.Count == 0)
				Destroy(gameObject);
		}
	}
	#endregion
	private void StartCreepGenerators()
	{
		Message creepGeneratorMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.CreateCreepGenerator);
		creepGeneratorMessage.AddUShort((ushort)creepGenerators.Count);
		ushort generatorId = 0;
		foreach (var item in creepGenerators)
		{
			creepGeneratorMessage.AddUShort((ushort)item.generatorData.generatorTeam);
			creepGeneratorMessage.AddUShort((ushort)item.generatorData.generatorLine);
			creepGeneratorMessage = HelperMethods.Instance.AddVector3(item.transform.position, creepGeneratorMessage);
			creepGeneratorMessage.AddUShort(generatorId);
			item.Init(generatorId, lobbyKey, this);
			generatorId++;
		}
		NetworkManager.Instance.SendMessageToAllUsersInLobby(creepGeneratorMessage, lobbyKey);
	}
	#region public Method
	public void AddNewPlayer(ushort _pId, string _userId, Teams _team)
	{
		playersInLobby.Add(_pId,
			new UserInLobbyData()
			{
				pId = _pId,
				player = new PlayerController()
				{
					playerTeam = _team,
					userId = _userId
				}
			});
		
	}
	public void UserInputManager(ushort pId, Message _inputMessage)
	{
		if (playersInLobby.TryGetValue(pId, out UserInLobbyData user))
			user.hero.PlayerInputMove(_inputMessage);
	}
	#endregion
}
