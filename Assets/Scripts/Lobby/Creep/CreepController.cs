using Dobeil;
using NetworkManagerModels;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreepController : MonoBehaviour
{
    [FoldoutGroup("Movement Info")] [ReadOnly] public Waypoint firstWaypoint;
    [FoldoutGroup("Movement Info")] [ReadOnly] public Waypoint CurrentWaypoint;
    [FoldoutGroup("Movement Info")] [SerializeField] private NavMeshAgent agent;
    [FoldoutGroup("Basic Data")] [SerializeField] private CreepControllerScriptableData creepData;
    public bool isAlive = true;
    [HideInInspector] public string creepId = "";
    private CreepState CurrentState;
    private Health health;
    private TargetData target = null;
    private string enemyTag = "";

    private int tick = 1;
    private float timer = 0;
    private float lastAttackTime = 0;
    private string lobbyKey = "";
    public void Init(string _lobbyKey = "", string _creepId = "")
	{
        isAlive = true;
        CurrentState = CreepState.Moving;
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
        if (_creepId != "")
            creepId = _creepId;
        health.Init(lobbyKey, creepId);
        gameObject.SetActive(true);

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
        //TODO Creep Dead Check please
        if (!health.isAlive || health.HpCount <= 0)
        {
            if (isAlive)
            {
                isAlive = false;
                agent.SetDestination(transform.position);
                //TODO Add Gold to killer 
                //AddGoldToKillers();
                CurrentState = CreepState.Dead;
                gameObject.SetActive(false);
            }
            return;
        }
        if (CurrentWaypoint != null && Vector3.Distance(transform.position, CurrentWaypoint.GetPosition()) <= agent.stoppingDistance && CurrentWaypoint.nextWaypoint != null)
            CurrentWaypoint = CurrentWaypoint.nextWaypoint;

		CurrentState = GetCurrentState();
		ExecuteAction(CurrentState);
		FaceTarget();
        timer += Time.deltaTime;
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
                return CreepState.Attacking;
            case CreepStateAction.MoveToEnemyHero:
                return CreepState.Attacking;
            case CreepStateAction.MoveToEnemyTower:
                return CreepState.Attacking;
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
                return CheckToMoveToEnemyByTag(allEnemyNear, "Creep", LayerMask.NameToLayer("Creep"));
            case CreepStateAction.MoveToEnemyHero:
                return CheckToMoveToEnemyByTag(allEnemyNear, "Player", LayerMask.NameToLayer("Player"));
            case CreepStateAction.MoveToEnemyTower:
                return CheckToMoveToEnemyByTag(allEnemyNear, "Tower", LayerMask.NameToLayer("Tower"));
            case CreepStateAction.AttackToEnemyCreep:
                return (CheckUnitIsNotNullAndAlive(target) && target.transform.tag.Contains("Creept"));
            case CreepStateAction.AttackToEnemyHero:
                return CheckUnitIsNotNullAndAlive(target) && target.transform.tag.Contains("Player");
            case CreepStateAction.AttackToEnemyTower:
                return CheckUnitIsNotNullAndAlive(target) && target.transform.tag.Contains("Tower");
            default:
                Debug.Log("Invalid State!!!");
                return false;
        }
    }

    private void ExecuteAction(CreepState newState, bool FromMessageHandler = false)
    {
		switch (newState)
		{
			case CreepState.Moving:
                agent.SetDestination(CurrentWaypoint.GetPosition());
				break;
			case CreepState.Attacking:
                if (CheckUnitIsNotNullAndAlive(target) && agent.isOnNavMesh)
                {
                    agent.SetDestination(target.transform.position);
                    float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

                    if (distanceToTarget < creepData.creeptAttackPlane)
                    {
                        if (timer - lastAttackTime > creepData.creeptDellay) 
						{
                            lastAttackTime = timer;
                            target.health.DecreaseHp(creepData.creepAttackDamage, creepData.creepDamageType, creepId);
						}
                    }
                }
                
				break;
			case CreepState.Dead:
				break;
			default:
				break;
		}
	}
    private void FaceTarget()
    {
        if (CheckUnitIsNotNullAndAlive(target))
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * creepData.rotationSpeed);
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
    private bool CheckToMoveToEnemyByTag(List<TargetData> allEnemyNear, string _tag, LayerMask layerMask)
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
    private bool CheckUnitIsNotNullAndAlive(TargetData unit)
    {
        return unit != null && unit.transform != null && unit.health != null && unit.health.isAlive;
    }
	#endregion


	#region Sync

    private void SendSyncMessage()
	{
        List<string> syncMessage = new List<string>();
        syncMessage.Add(((int)ServerToClientId.SyncCreep).ToString());
        syncMessage.Add(creepId.ToString());
        syncMessage.Add(transform.position.ToString());
        syncMessage.Add(((int)CurrentState).ToString());
		switch (CurrentState)
		{
			case CreepState.Moving:
                syncMessage.Add(CurrentWaypoint.GetPosition().ToString());
				break;
			case CreepState.Attacking:
                syncMessage.Add(target.transform.position.ToString());
				break;
			case CreepState.Dead:
				break;
			default:
				break;
		}
        OnlineServer.Instance.BroadCastMessageInLobby(Utility.EnCodeMessage(syncMessage), lobbyKey, SendMessageProtocol.UDP);
	}
	#endregion
}
