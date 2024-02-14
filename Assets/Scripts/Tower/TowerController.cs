using Dobeil;
using NetworkManagerModels;
using Riptide;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class TowerController : ObjectInGameBase
{
	[EnumToggleButtons] [HideLabel] public TowersLine line;
	[SerializeField] private TowerControllerScriptableObject towerData;

	private TowerState currentState;

	private DateTime lastAttackTime;
	public override void Init(LobbyManager _manager, ushort _elementId)
	{
		data = towerData;
		base.Init(_manager, _elementId);
		lastAttackTime = DateTime.Now;
		StartCoroutine(TowerDecesion());
	}
	private IEnumerator TowerDecesion()
	{
		while (health.isAlive)
		{
			currentState = GetCurrentState();
			ExecuteAction(currentState);
			yield return new WaitForSeconds(towerData.towerDecesionDellay);
		}
	}
	private TowerState GetCurrentState()
	{
		TowerScore score = towerData.towerActionScore.GetType().GetField(currentState.ToString()).GetValue(towerData.towerActionScore) as TowerScore;
		TowerStateAction bestAction = TowerStateAction.DoNothing;

		float bestScore = score.DoNothing;
		List<ObjectInGameBase> allEnemyNear = FindAllTarget();
		foreach (TowerStateAction action in Enum.GetValues(typeof(TowerStateAction)))
		{
			if (action == TowerStateAction.DoNothing)
				continue;

			float actionScore = (float)score.GetType().GetField(action.ToString()).GetValue(score);
			if (actionScore > bestScore)
			{
				if (!IsActionValid(allEnemyNear, action))
				{
					continue;
				}
				bestScore = actionScore;
				bestAction = action;
			}

		}
		return bestAction switch
		{
			TowerStateAction.DoNothing => TowerState.Idle,
			TowerStateAction.AttackToEnemyCreep => TowerState.Attacking,
			TowerStateAction.AttackToEnemyHero => TowerState.Attacking,
			_ => TowerState.Idle,
		};
	}
	private bool IsActionValid(List<ObjectInGameBase> allEnemyNear, TowerStateAction action)
	{
		
		switch (action)
		{
			case TowerStateAction.DoNothing:
				return allEnemyNear.Count == 0;
			case TowerStateAction.AttackToEnemyCreep:
				return CheckAttackActionWithTag(allEnemyNear, GameElement.Creep.ToString());
			case TowerStateAction.AttackToEnemyHero:
				return CheckAttackActionWithTag(allEnemyNear, GameElement.Hero.ToString());
			default:
				Debug.Log("Invalid State!!!");
				return false;
		}
	}
	private void ExecuteAction(TowerState newState)
	{
		if (newState == TowerState.Attacking && CheckTargeIsAvaiable(target))
		{
			TimeSpan deltaTime = DateTime.Now - lastAttackTime;
			if (deltaTime.Seconds >= towerData.attackDellay)
			{
				lastAttackTime = DateTime.Now;
				target.DecreaseHp(towerData.baseAttackDamage, DamageType.Physical, GameElement.Structure.ToString() + elementId);
				Message towerShootMessage = Message.Create(MessageSendMode.Unreliable, ServerToClientId.TowerShoot);

				towerShootMessage.AddUShort(elementId);
				towerShootMessage.AddUShort(target.health.id);
				towerShootMessage.AddUShort((ushort)target.health.unitType);
				NetworkManager.Instance.SendMessageToAllUsersInLobby(towerShootMessage, manager.lobbyKey);
			}
		}
	}
	private bool CheckAttackActionWithTag(List<ObjectInGameBase> targets, string _tag)
	{
		return FindNearestEnemy(targets, _tag) != null;
	}
	
}
