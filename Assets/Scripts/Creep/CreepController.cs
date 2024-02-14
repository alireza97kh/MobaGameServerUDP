using Dobeil;
using NetworkManagerModels;
using Riptide;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TreeEditor;
using UnityEngine;
using UnityEngine.AI;

public class CreepController : ObjectInGameBase
{
    [FoldoutGroup("Movement Info")] [ReadOnly] public Waypoint firstWaypoint;
    [FoldoutGroup("Movement Info")] [ReadOnly] public Waypoint CurrentWaypoint;
    [FoldoutGroup("Basic Data")] [SerializeField] private CreepControllerScriptableData creepData;
    private CreepState CurrentState;

    private int tick = 1;

    private float creepAttackTimer = 0;
    public override void Init(LobbyManager _manager, ushort _elementId)
	{
        if (_elementId != 0)
        {
			data = creepData;
			base.Init(_manager, _elementId);
		}
        
        CurrentState = CreepState.Idle;
        agent.Warp(firstWaypoint.GetPosition());
        CurrentWaypoint = firstWaypoint;
        gameObject.SetActive(true);
        StartCoroutine(CreepDecesion());
    }

    private IEnumerator CreepDecesion() 
    {
        while (isAlive)
        {
			if (CheckCurrentWaypoint())
				CurrentWaypoint = CurrentWaypoint.nextWaypoint;
			CurrentState = GetCurrentState();
            ExecuteAction(CurrentState);
			yield return new WaitForSecondsRealtime(creepData.creeptDecesionDellay);
        }
    }
    #region Unity Callback
    private void Update()
    {
		if (health == null)
            return;
        if (!health.isAlive || health.HpCount <= 0)
        {
            if (isAlive)
            {
                isAlive = false;
                agent.SetDestination(transform.position);
                CurrentState = CreepState.Dead;
                SendSyncMessage();
                tick = 0;
			}
            return;
        }
        tick++;
        if (tick % creepData.sendSyncTick == 0)
            SendSyncMessage();
    }
    #endregion
    #region Private Function
    private CreepState GetCurrentState()
    {
        CreepScore score = creepData.score.GetType().GetField(CurrentState.ToString()).GetValue(creepData.score) as CreepScore;

        CreepStateAction bestAction = CreepStateAction.DoNothing;

        float bestScore = score.DoNothing;
		List<ObjectInGameBase> allEnemyNear = FindAllTarget();
		foreach (CreepStateAction action in Enum.GetValues(typeof(CreepStateAction)))
        {
            if (action == CreepStateAction.DoNothing)
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
        switch (bestAction)
        {
            case CreepStateAction.DoNothing:
                return CurrentState;
            case CreepStateAction.KeepMoving:
                return CreepState.Moving;
            case CreepStateAction.MoveToEnemyCreep:
                target = FindNearestEnemy(allEnemyNear, GameElement.Creep.ToString());
                return CreepState.MoveToTarget;
            case CreepStateAction.MoveToEnemyHero:
				target = FindNearestEnemy(allEnemyNear, GameElement.Hero.ToString());
				return CreepState.MoveToTarget;
            case CreepStateAction.MoveToEnemyTower:
				target = FindNearestEnemy(allEnemyNear, GameElement.Structure.ToString());
				return CreepState.MoveToTarget;
            case CreepStateAction.AttackToEnemyCreep:
                return CreepState.Attacking;
            case CreepStateAction.AttackToEnemyHero:
                return CreepState.Attacking;
            case CreepStateAction.AttackToEnemyTower:
                return CreepState.Attacking;
            default:
                Debug.Log("Invalid Action !!!");
                return CurrentState;
        }

    }

    private bool IsActionValid(List<ObjectInGameBase> allEnemyNear, CreepStateAction action)
    {


        switch (action)
        {
            case CreepStateAction.DoNothing:
                return true;
            case CreepStateAction.KeepMoving:
                return allEnemyNear.Count == 0 && CurrentWaypoint != null;
            case CreepStateAction.MoveToEnemyCreep:
                return CheckToMoveToEnemyByTag(allEnemyNear, GameElement.Creep.ToString());
            case CreepStateAction.MoveToEnemyHero:
                return CheckToMoveToEnemyByTag(allEnemyNear, GameElement.Hero.ToString());
            case CreepStateAction.MoveToEnemyTower:
                return CheckToMoveToEnemyByTag(allEnemyNear, GameElement.Structure.ToString());
            case CreepStateAction.AttackToEnemyCreep:
                return CheckAttackActionWithTag(target, GameElement.Creep.ToString());
            case CreepStateAction.AttackToEnemyHero:
                return CheckAttackActionWithTag(target, GameElement.Hero.ToString());
            case CreepStateAction.AttackToEnemyTower:
                return CheckAttackActionWithTag(target, GameElement.Structure.ToString());
            default:
                Debug.Log("Invalid State!!!");
                return false;
        }
    }

    private void ExecuteAction(CreepState newState)
    {
        creepAttackTimer = (newState == CreepState.Attacking) ? creepAttackTimer + creepData.creeptDecesionDellay : 0;
		switch (newState)
		{
			case CreepState.Moving:
                agent.SetDestination(CurrentWaypoint.GetPosition());
				break;
            case CreepState.MoveToTarget:
				if (agent.isOnNavMesh && CheckTargeIsAvaiable(target))
					agent.SetDestination(target.transform.position);
                break;
			case CreepState.Attacking:
                if (creepAttackTimer >= creepData.attackDellay)
                {
                    creepAttackTimer = 0;
					if (agent.isOnNavMesh)
					{
						if (CheckTargeIsAvaiable(target))
							target.health.DecreaseHp(creepData.baseAttackDamage, DamageType.Physical, GameElement.Creep.ToString() + elementId);
					}
				}
				break;
			case CreepState.Dead:
				break;
			default:
				break;
		}
	}
    
    private bool CheckToMoveToEnemyByTag(List<ObjectInGameBase> targets, string _tag)
    {
        return FindNearestEnemy(targets, _tag) != null;
    }

    private bool CheckAttackActionWithTag(ObjectInGameBase target, string _tag) 
    {
        return (CheckTargeIsAvaiable(target) && target.transform.tag.Contains(_tag));
	}
	private bool CheckCurrentWaypoint()
	{
		return (CurrentWaypoint != null && CurrentWaypoint.nextWaypoint != null && Vector3.Distance(transform.position, CurrentWaypoint.GetPosition()) <= agent.stoppingDistance);
	}
    #endregion


    #region Sync
	private void SendSyncMessage()
	{
        Message syncCreepMessage = Message.Create(MessageSendMode.Unreliable, ServerToClientId.SyncCreep);
        syncCreepMessage.AddUShort(elementId);
        syncCreepMessage = HelperMethods.Instance.AddVector3(transform.position, syncCreepMessage);
		syncCreepMessage.AddUShort((ushort)CurrentState);
        NetworkManager.Instance.SendMessageToAllUsersInLobby(syncCreepMessage, manager.lobbyKey);
	}

	#endregion
}
