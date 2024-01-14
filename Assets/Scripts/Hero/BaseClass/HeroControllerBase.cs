using Dobeil;
using NetworkManagerModels;
using Riptide;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using ReadOnlyAttribute = Sirenix.OdinInspector.ReadOnlyAttribute;
public abstract class HeroControllerBase : MonoBehaviour
{
    public string heroName;
    public Sprite heroIcon;
	[ReadOnly] public ushort pId;
    [ReadOnly] public ushort heroId = 0;
    [ReadOnly] public string userId;

	[ReadOnly] public Teams team;
	[ReadOnly] public int xp = 0;
    [ReadOnly] public int gold = 0;
    public int sendSyncTick = 10;
	public LobbyManager lobbyManager;

	public NavMeshAgent agent;
	public Health health;
    public Dictionary<ushort, AbilityBase> heroLevels = new Dictionary<ushort, AbilityBase>();

	protected int timer = 0;

    public virtual void Init()
    {
        Message heroInitMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.CreateHero);
        heroInitMessage = HelperMethods.Instance.AddVector3(transform.position, heroInitMessage);
		heroInitMessage.AddBool(team == Teams.Team1);
		heroInitMessage.AddUShort(pId);
		heroInitMessage.AddUShort(heroId);
		heroInitMessage.AddString(lobbyManager.playersInLobby[pId].player.userId);
		NetworkManager.Instance.SendMessageToAllUsersInLobby(heroInitMessage, lobbyManager.lobbyKey);
	}

    public void OnSelectedHero(ushort _heroId, ushort _pId, LobbyManager _manager)
    {
        heroId = _heroId;
        lobbyManager = _manager;
        pId = _pId;
    }

    public virtual void PlayerInputMove(Vector2 input)
    {
		Vector3 movement = new(-input.y, 0f, input.x);
		agent.SetDestination(transform.position + movement);
	}

    protected virtual void Update()
    {
		if (lobbyManager != null && lobbyManager.gameStarted)
		{
			timer++;
			if (timer > sendSyncTick)
			{
				SendSyncMessage();
				timer = 0;
			}
		}
	}
	private void SendSyncMessage()
	{
		Message syncMessage = Message.Create(MessageSendMode.Unreliable, ServerToClientId.Sync);
		syncMessage.AddUShort(pId);
		syncMessage = HelperMethods.Instance.AddVector3(transform.position, syncMessage);
		NetworkManager.Instance.SendMessageToAllUsersInLobby(syncMessage, lobbyManager.lobbyKey);

	}
}
