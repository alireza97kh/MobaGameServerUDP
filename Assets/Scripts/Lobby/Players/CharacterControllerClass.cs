using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NetworkManagerModels;
using TreeEditor;
using static UnityEditor.Progress;

public class CharacterControllerClass : MonoBehaviour
{
	public ushort pId;
	public LobbyManager manager;
	public string selectedHeroId = "";
	public string userId = "";
    public ushort characterId;
	public bool isTeam1 = false;
    public NavMeshAgent character;
	public int gold = 0;
	public int xp = 0;
	public int tick = 10;

	private Vector3 lastSendPosition = Vector3.zero;
	public void Init()
	{
		Message initMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.CreateHero);
		initMessage = HelperMethods.Instance.AddVector3(transform.position, initMessage);
		initMessage.AddBool(isTeam1);
		initMessage.AddUShort(characterId);
		initMessage.AddString(selectedHeroId);
		initMessage.AddString(userId);
		NetworkManager.Instance.SendMessageToAllUsersInLobby(initMessage, manager.lobbyKey);
	}
	public void OnSelectedHero(string heroId, string _userId, ushort _pId, LobbyManager _manager)
	{
		selectedHeroId = heroId;
		userId = _userId;
		pId = _pId;
		manager = _manager;
	}

	public void PlayerInputController(Vector2 input)
	{
		Vector3 movement = new(-input.y, 0f, input.x);
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

	#region Private Functions
	int timer = 0;
	private void FixedUpdate()
	{
		if (manager != null && manager.gameStarted)
		{
			timer++;
			if (timer > tick)
			{
				SendSyncMessage();
				timer = 0;
			}
		}
	}
	private void SendSyncMessage()
	{
        if ((lastSendPosition - transform.position).magnitude > 0.05f)
        {
			lastSendPosition = transform.position;
			Message syncMessage = Message.Create(MessageSendMode.Unreliable, ServerToClientId.Sync);
			syncMessage.AddUShort(pId);
			syncMessage = HelperMethods.Instance.AddVector3(transform.position, syncMessage);
			NetworkManager.Instance.SendMessageToAllUsersInLobby(syncMessage, manager.lobbyKey);
		}
        
	}
	#endregion
}
