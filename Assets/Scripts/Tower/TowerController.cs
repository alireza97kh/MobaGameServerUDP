using Dobeil;
using NetworkManagerModels;
using Riptide;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class TowerController : MonoBehaviour
{
	[EnumToggleButtons] [HideLabel] public TowersTeam team;
	[EnumToggleButtons] [HideLabel] public TowersLine line;
	[SerializeField] private Health towerHealth;
	[SerializeField] private TowerControllerScriptableObject towerData;

	private TowerState currentState;
	private TargetData target = null;
	private string enemyTag = "";
	[ReadOnly] public ushort id;
	private string lobbyKey;

	private DateTime lastAttackTime;
	public void Init(string _lobbyKey, ushort _id)
	{
		lastAttackTime = DateTime.Now;
		tag = team.ToString() + "Tower";
		if (enemyTag == "")
			SetEnemyTag();
		id = _id;
		lobbyKey = _lobbyKey;
		towerHealth.maxHp = towerData.towerMaxHp;
		towerHealth.Init(lobbyKey, id, CurrentUnitHealthType.Tower);
		StartCoroutine(TowerDecesion());
	}
	private void SetEnemyTag()
	{
		if (team == TowersTeam.Team1)
			enemyTag = "Team2";
		else
			enemyTag = "Team1";
	}
	private IEnumerator TowerDecesion()
	{
		while (towerHealth.isAlive)
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
		foreach (TowerStateAction action in Enum.GetValues(typeof(TowerStateAction)))
		{
			if (action == TowerStateAction.DoNothing)
				continue;

			float actionScore = (float)score.GetType().GetField(action.ToString()).GetValue(score);
			if (actionScore > bestScore)
			{
				if (!IsActionValid(action))
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
	private bool IsActionValid(TowerStateAction action)
	{
		List<TargetData> allEnemyNear = FindAllTarget();
		switch (action)
		{
			case TowerStateAction.DoNothing:
				return allEnemyNear.Count == 0;
			case TowerStateAction.AttackToEnemyCreep:
				return CheckAttackActionWithTag("Creep", allEnemyNear);
			case TowerStateAction.AttackToEnemyHero:
				return CheckAttackActionWithTag("Hero", allEnemyNear);
			default:
				Debug.Log("Invalid State!!!");
				return false;
		}
	}
	private void ExecuteAction(TowerState newState)
	{
		if (newState == TowerState.Attacking && CheckUnit(target, enemyTag))
		{
			TimeSpan deltaTime = DateTime.Now - lastAttackTime;
			if (deltaTime.Seconds >= towerData.towerAttackDellay)
			{
				lastAttackTime = DateTime.Now;
				target.health.DecreaseHp(towerData.towerAttackDamage, towerData.towerDamageType, "Tower" + id);
				Message towerShootMessage = Message.Create(MessageSendMode.Unreliable, ServerToClientId.TowerShoot);

				towerShootMessage.AddUShort(id);
				towerShootMessage.AddUShort(target.health.id);
				towerShootMessage.AddUShort((ushort)target.health.unitType);
				NetworkManager.Instance.SendMessageToAllUsersInLobby(towerShootMessage, lobbyKey);
			}
		}
	}
	private bool CheckAttackActionWithTag(string _tag, List<TargetData> allEnemyNear)
	{
		if ((!CheckUnit(target, enemyTag) || !CheckTargeIsInAttackRange(target)) && allEnemyNear.Count > 0)
		{
			int index = -1;
			float maxDistance = towerData.towerAround;
			for (int i = 0; i < allEnemyNear.Count; i++)
			{
				if (allEnemyNear[i].transform.tag.Contains(_tag) && Vector3.Distance(transform.position, allEnemyNear[i].transform.position) < maxDistance)
				{
					index = i;
				}
			}
			if (index >= 0 && index < allEnemyNear.Count)
				target = allEnemyNear[index];
		}
		if (CheckUnit(target, _tag))
		{
			if (!CheckTargeIsInAttackRange(target))
			{
				target = null;
				return false;
			}
			return true;
		}
		else if (target != null)
			target = null;
		return false;
	}
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
		return (distanceToTarget < towerData.towerAround);

	}
	private List<TargetData> FindAllTarget()
	{
		Collider[] col = Physics.OverlapSphere(transform.position, towerData.towerAround);
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
		return enemyUnitsInRange;
	}
}
