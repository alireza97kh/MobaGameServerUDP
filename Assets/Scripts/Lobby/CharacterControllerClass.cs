using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NetworkManagerModels;

public class CharacterControllerClass : MonoBehaviour
{
	public int pId;
	public LobbyManager manager;
	public string selectedHeroId = "";
	public string userId = "";
    public int characterId;
	public bool isTeam1 = false;
    public NavMeshAgent character;
	public int gold = 0;
	public int xp = 0;
	public void OnSelectedHero(string heroId, string _userId, int _pId, LobbyManager _manager)
	{
		selectedHeroId = heroId;
		userId = _userId;
		pId = _pId;
		manager = _manager;
	}

	public void Init()
	{
		List<string> list = new List<string>();
		list.Add($"{(int)ServerToClientId.CreateHero}");
        list.Add(transform.position.ToString());
		list.Add(isTeam1.ToString());
		list.Add(characterId.ToString());
		list.Add(selectedHeroId.ToString());
		list.Add(userId.ToString());
		OnlineServer.Instance.BroadCastMessageInLobby(Utility.EnCodeMessage(list), manager.lobbyKey, Dobeil.SendMessageProtocol.TCP);
	}

	public void PlayerInputController(Vector2 input)
	{
		Vector3 movement = new Vector3(-input.y, 0f, input.x);
		character.SetDestination(transform.position + movement);
	}

	public void AddGold(int _gold)
	{
		gold += _gold;
	}
	public void AddXp(int _xp)
	{
		xp += _xp;
	}
}
