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

public class CreepController : MonoBehaviour
{
    [FoldoutGroup("Movement Info")] [ReadOnly] public Waypoint firstWaypoint;
    [FoldoutGroup("Movement Info")] [ReadOnly] public Waypoint CurrentWaypoint;
    [FoldoutGroup("Movement Info")] [SerializeField] private NavMeshAgent agent;
    [FoldoutGroup("Basic Data")] [SerializeField] private CreepControllerScriptableData creepData;
    public bool isAlive = true;
    [HideInInspector] public ushort creepId = 0;
    private CreepState CurrentState;
    private Health health;
    private TargetData target = null;
    private string enemyTag = "";
    private int tick = 1;
    private string lobbyKey = "";

    private float creepAttackTimer = 0;
    public void Init(string _lobbyKey = "", ushort _creepId = 0)
	{
        isAlive = true;
        CurrentState = CreepState.Idle;
		if (agent == null)
            agent = GetComponent<NavMeshAgent>();
		if (health == null)
            health = GetComponent<Health>();
        agent.Warp(firstWaypoint.GetPosition());
        CurrentWaypoint = firstWaypoint;
		if (enemyTag == "")
            SetEnemyTag();
		if (_lobbyKey != "")
            lobbyKey = _lobbyKey;
        if (_creepId != 0)
            creepId = _creepId;
        health.maxHp = creepData.creepMaxHp;
		health.Init(lobbyKey, creepId, CurrentUnitHealthType.Creep);
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
        //File.AppendAllText(Application.dataPath + "Creep.txt", $"Crepp{creepId} Send Data {countOfSendBytes} in {tick} and {countOfSendMessage} Messages\n");
    }

    private void SetEnemyTag()
    {
        if (transform.tag.Contains("Team1"))
            enemyTag = "Team2";
        else
            enemyTag = "Team1";
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
        if (tick % creepData.sendSyncMessageTick == 0)
            SendSyncMessage();
    }
    #endregion
    #region Private Function
    private CreepState GetCurrentState()
    {
        CreepScore score = creepData.score.GetType().GetField(CurrentState.ToString()).GetValue(creepData.score) as CreepScore;

        CreepStateAction bestAction = CreepStateAction.DoNothing;

        float bestScore = score.DoNothing;
        foreach (CreepStateAction action in Enum.GetValues(typeof(CreepStateAction)))
        {
            if (action == CreepStateAction.DoNothing)
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
        switch (bestAction)
        {
            case CreepStateAction.DoNothing:
                return CurrentState;
            case CreepStateAction.KeepMoving:
                return CreepState.Moving;
            case CreepStateAction.MoveToEnemyCreep:
                return CreepState.MoveToTarget;
            case CreepStateAction.MoveToEnemyHero:
                return CreepState.MoveToTarget;
            case CreepStateAction.MoveToEnemyTower:
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

    private bool IsActionValid(CreepStateAction action)
    {

        List<TargetData> allEnemyNear = FindAllTarget();

        switch (action)
        {
            case CreepStateAction.DoNothing:
                return true;
            case CreepStateAction.KeepMoving:
                return allEnemyNear.Count == 0 && CurrentWaypoint != null;
            case CreepStateAction.MoveToEnemyCreep:
                return CheckToMoveToEnemyByTag(allEnemyNear, "Creep");
            case CreepStateAction.MoveToEnemyHero:
                return CheckToMoveToEnemyByTag(allEnemyNear, "Hero");
            case CreepStateAction.MoveToEnemyTower:
                return CheckToMoveToEnemyByTag(allEnemyNear, "Tower");
            case CreepStateAction.AttackToEnemyCreep:
                return CheckAttackActionWithTag(target, "Creep");
            case CreepStateAction.AttackToEnemyHero:
                return CheckAttackActionWithTag(target, "Hero");
            case CreepStateAction.AttackToEnemyTower:
                return CheckAttackActionWithTag(target, "Tower");
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
				if (agent.isOnNavMesh)
					agent.SetDestination(target.transform.position);
                break;
			case CreepState.Attacking:
                if (creepAttackTimer >= creepData.creeptAttackDellay)
                {
                    creepAttackTimer = 0;
					if (agent.isOnNavMesh)
					{
						float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
						if (distanceToTarget < creepData.creeptAttackPlane)
							target.health.DecreaseHp(creepData.creepAttackDamage, creepData.creepDamageType, "Creep" + creepId);
					}
				}
				break;
			case CreepState.Dead:
				break;
			default:
				break;
		}
	}
    private List<TargetData> FindAllTarget()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, creepData.creepDetection);
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
    private bool CheckToMoveToEnemyByTag(List<TargetData> allEnemyNear, string _tag)
    {
        if (allEnemyNear.Count > 0)
        {
            int indexOfNearestEnemy = -1;
            float maxDist = creepData.creepDetection;
            if (CheckUnitIsNotNullAndAlive(target) && target.transform.tag.Contains(_tag))
                maxDist = Vector3.Distance(transform.position, target.transform.position);
            for (int i = 0; i < allEnemyNear.Count; i++)
            {
                if (allEnemyNear[i].transform.tag.Contains(_tag) && Vector3.Distance(transform.position, allEnemyNear[i].transform.position) < maxDist)
                {
                    indexOfNearestEnemy = i;
                }
            }
            if (indexOfNearestEnemy >= 0 && indexOfNearestEnemy < allEnemyNear.Count)
            {
                target = allEnemyNear[indexOfNearestEnemy];
                return true;
            }
            else if (maxDist != creepData.creepDetection)
            {
                // Our target is the nearest
                return true;
            }
            return false;
        }
        return false;
    }

    private bool CheckAttackActionWithTag(TargetData target, string _tag) 
    {
        return (CheckUnitIsNotNullAndAlive(target) && target.transform.tag.Contains(_tag) && CheckTargeIsInAttackRange(target));
	}

    private bool CheckUnitIsNotNullAndAlive(TargetData unit)
    {
        return unit != null && unit.transform != null && unit.health != null && unit.health.isAlive;
    }

    private bool CheckTargeIsInAttackRange(TargetData unit)
    {
		float distanceToTarget = Vector3.Distance(transform.position, unit.transform.position);
        return (distanceToTarget < creepData.creeptAttackPlane);

	}
	private bool CheckCurrentWaypoint()
	{
		return (CurrentWaypoint != null && Vector3.Distance(transform.position, CurrentWaypoint.GetPosition()) <= agent.stoppingDistance && CurrentWaypoint.nextWaypoint != null);

	}
    #endregion


    #region Sync
    private int countOfSendBytes = 0;
    private int countOfSendMessage = 0;
	private void SendSyncMessage()
	{
        Message syncCreepMessage = Message.Create(MessageSendMode.Unreliable, ServerToClientId.SyncCreep);
        syncCreepMessage.AddUShort(creepId);
        syncCreepMessage = HelperMethods.Instance.AddVector3(transform.position, syncCreepMessage);
		syncCreepMessage.AddUShort((ushort)CurrentState);
        countOfSendBytes += syncCreepMessage.BytesInUse;
        countOfSendMessage++;
        NetworkManager.Instance.SendMessageToAllUsersInLobby(syncCreepMessage, lobbyKey);
	}

	#endregion
}
