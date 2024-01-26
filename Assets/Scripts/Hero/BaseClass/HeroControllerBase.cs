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
using UnityEngine.Windows;
public abstract class HeroControllerBase : MonoBehaviour
{
	[SerializeField] protected HeroControllerBaseData heroData;
	[ReadOnly] public ushort pId;
    [ReadOnly] public ushort heroId = 0;
    [ReadOnly] public string userId;

	[ReadOnly] public Teams team;
	[ReadOnly] public int xp = 0;
    [ReadOnly] public int gold = 0;

    public int sendSyncTick = 10;

	public NavMeshAgent agent;
	public Health health;
	public AbilityExecutor ability;

	protected LobbyManager lobbyManager;
	protected int timer = 0;


	protected TargetData target;
	protected string enemyTag = "";
    public virtual void Init()
    {
		tag = team.ToString() + "Hero";
		enemyTag = team == Teams.Team1 ? "Team2" : "Team1";
		health.maxHp = heroData.heroMaxHp;
		health.Init(lobbyManager.lobbyKey, pId, CurrentUnitHealthType.Hero);
		SendCreateMessage();
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


	#region private Methods
	
	#endregion


	#region Send Message
	private void SendCreateMessage()
	{
		Message heroInitMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.CreateHero);
		heroInitMessage = HelperMethods.Instance.AddVector3(transform.position, heroInitMessage);
		heroInitMessage.AddBool(team == Teams.Team1);
		heroInitMessage.AddUShort(pId);
		heroInitMessage.AddUShort(heroId);
		heroInitMessage.AddString(lobbyManager.playersInLobby[pId].player.userId);
		NetworkManager.Instance.SendMessageToAllUsersInLobby(heroInitMessage, lobbyManager.lobbyKey);
	}
	private void SendSyncMessage()
	{
		Message syncMessage = Message.Create(MessageSendMode.Unreliable, ServerToClientId.Sync);
		syncMessage.AddUShort(pId);
		syncMessage = HelperMethods.Instance.AddVector3(transform.position, syncMessage);
		NetworkManager.Instance.SendMessageToAllUsersInLobby(syncMessage, lobbyManager.lobbyKey);

	}
	#endregion

	#region public Methods
	public void OnSelectedHero(ushort _heroId, ushort _pId, LobbyManager _manager)
	{
		heroId = _heroId;
		lobbyManager = _manager;
		pId = _pId;
	}
	
	#endregion

	#region Input Controller
	protected virtual void BaseAttack()
	{
		if (true)
		{

		}
	}
	public virtual void PlayerInputMove(Message _heroInput)
	{
		HeroInputType heroInputType = (HeroInputType)_heroInput.GetUShort();

		switch (heroInputType)
		{
			case HeroInputType.Movement:
				Vector2 movementInput = HelperMethods.Instance.GetVector2(_heroInput);
				Vector3 movement = new(-movementInput.y, 0f, movementInput.x);
				agent.SetDestination(transform.position + movement);
				break;
			case HeroInputType.BaseAttack:
				Debug.LogError("User send Base Attack input");
				break;
			case HeroInputType.Ability:
				Debug.LogError("User send Ability input");
				break;
			case HeroInputType.Item:
				break;
			default:
				break;
		}
	}
	#endregion

	#region	protected Methods
	private bool CheckUnit(TargetData unit, string _tag)
	{
		return unit != null &&
			unit.transform != null &&
			unit.health != null &&
			unit.health.isAlive &&
			unit.transform.tag.Contains(_tag);
	}
	private bool CheckTargeIsInAttackRange(TargetData unit)
	{
		float distanceToTarget = Vector3.Distance(transform.position, unit.transform.position);
		return (distanceToTarget < heroData.heroAttackArea);
	}

	private List<TargetData> FindAllTarget()
	{
		Collider[] col = Physics.OverlapSphere(transform.position, heroData.heroAttackArea);
		List<TargetData> enemyUnitsInRange = new List<TargetData>();
		foreach (Collider item in col)
		{
			if (item.tag != tag && item.tag.Contains(enemyTag))
			{
				Health TempHealth = item.GetComponent<Health>();
				if (TempHealth != null && TempHealth.isAlive)
					enemyUnitsInRange.Add(new TargetData(item.transform, TempHealth));
			}
		}
		col[0].GetInstanceID();
		return enemyUnitsInRange;
	}
	#endregion
}
