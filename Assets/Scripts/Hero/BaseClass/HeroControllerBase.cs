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
using System;
using TreeEditor;
using TMPro;
public abstract class HeroControllerBase : ObjectInGameBase
{
	[SerializeField] protected HeroControllerBaseData heroData;
    [ReadOnly] public ushort heroId = 0;
    [ReadOnly] public string userId;

	[ReadOnly] public int xp = 0;
    [ReadOnly] public int gold = 0;


	public AbilityExecutor ability;

	protected int timer = 0;

	private DateTime lastAttackTime;

    public override void Init(LobbyManager _manager, ushort _elementId)
    {
		data = heroData;
		base.Init(_manager, _elementId);
		lastAttackTime = DateTime.Now;
		SendCreateMessage();
	}

    protected virtual void Update()
    {
		if (manager != null && manager.gameStarted)
		{
			timer++;
			if (timer > data.sendSyncTick)
			{
				SendSyncMessage();
				timer = 0;
			}
		}
	}

	#region Send Message
	private void SendCreateMessage()
	{
		Message heroInitMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.CreateHero);
		heroInitMessage = HelperMethods.Instance.AddVector3(transform.position, heroInitMessage);
		heroInitMessage.AddBool(team == Teams.Team1);
		heroInitMessage.AddUShort(elementId);
		heroInitMessage.AddUShort(heroId);
		heroInitMessage.AddString(manager.playersInLobby[elementId].player.userId);
		NetworkManager.Instance.SendMessageToAllUsersInLobby(heroInitMessage, manager.lobbyKey);
	}
	private void SendSyncMessage()
	{
		Message syncMessage = Message.Create(MessageSendMode.Unreliable, ServerToClientId.Sync);
		syncMessage.AddUShort(elementId);
		syncMessage = HelperMethods.Instance.AddVector3(transform.position, syncMessage);
		NetworkManager.Instance.SendMessageToAllUsersInLobby(syncMessage, manager.lobbyKey);

	}
	#endregion

	#region public Methods
	public void OnSelectedHero(ushort _heroId, ushort _pId, LobbyManager _manager)
	{
		heroId = _heroId;
		manager = _manager;
		elementId = _pId;
	}
	
	#endregion

	#region Input Controller
	protected virtual void BaseAttack()
	{
		if ((DateTime.Now - lastAttackTime).Seconds < heroData.attackDellay)
			return;
		lastAttackTime = DateTime.Now;
		List<ObjectInGameBase> allTargets = FindAllTarget();
		if (allTargets.Count > 0)
		{
			if (CheckTargeIsAvaiable(FindNearestEnemy(allTargets)))
				target.DecreaseHp(heroData.baseAttackDamage, DamageType.Physical, "hero" + heroId);
		}

		Message heroAttackMessge = Message.Create(MessageSendMode.Reliable, ServerToClientId.HeroAttack);

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
				BaseAttack();
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
}
