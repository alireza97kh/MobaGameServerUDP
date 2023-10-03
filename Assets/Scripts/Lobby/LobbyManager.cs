using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkManagerModels;
using Riptide;
using Riptide.Utils;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance = null;

    public string lobbyKey;
    public bool isFull = false;
    public int maxCountOfUser = 1;
    public Transform team1Parent;
    public Transform team2Parent;
    public CharacterControllerClass characterPrefab;
    public DictionaryWithEvent<ushort, CharacterControllerClass> usersInThisLobby = new DictionaryWithEvent<ushort, CharacterControllerClass>();


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
            StartGame();
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
            StartGame();
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
            Debug.Log("All Hero Has been Selected ");
            Message lobbyStartGameMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.AllHeroSelected);
			foreach (var item in usersInThisLobby)
			{
                item.Value.Init();
			}

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
		}
	}
	#endregion


	private void StartGame()
	{
        Debug.LogError("Game Started");
	}
    public void AddNewPlayer(ushort playerId, bool isTeam1)
	{
        CharacterControllerClass newUser = Instantiate(characterPrefab, isTeam1 ? team1Parent : team2Parent);
        newUser.characterId = playerId;
        usersInThisLobby.Add(playerId, newUser);

	}

    #region MessageHandlers
  //  public static void OnUsersSelectedHero(ushort fromClientId, Message message)
  //  {
		//if (Instance.usersInThisLobby.ContainsKey(fromClientId))
		//{
  //          string heroId = message.GetString();
  //          string userId = message.GetString();
  //          Debug.LogError($"Selected Hero {heroId}  by {userId}");
  //          Instance.usersInThisLobby[fromClientId].OnSelectedHero(heroId, userId);
  //          Instance.numberOfSelectedHero++;
		//	if (Instance.numberOfSelectedHero == Instance.maxCountOfUser)
		//	{
  //              Message msg = Message.Create(MessageSendMode.Reliable, ServerToClientId.AllHeroSelected);
  //              List<UsersHeroInLobby> allUsersHeroData = new List<UsersHeroInLobby>();
				
  //              foreach (var item in Instance.usersInThisLobby)
  //                  allUsersHeroData.Add(new UsersHeroInLobby(item.Value.userId.ToString(), item.Value.selectedHeroId));

  //              msg.AddString(JsonUtility.ToJson(new UsersHeroInLobbyList(allUsersHeroData)));
  //              NetworkManager.Instance.SendMessageToAllUsersInLobby(msg, Instance.lobbyKey);
		//	}
		//}
  //  }
	#endregion

	#region Private Functions
	#endregion
}
