using Dobeil;
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
    [HideInInspector] public int creepId = -1;
    private CreepState CurrentState;
    private Health health;
    private TargetData target = null;
    private string enemyTag = "";


    public void Init()
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
        if (!health.isAlive || health.hpCount <= 0)
        {
            if (isAlive)
            {
                isAlive = false;
                agent.SetDestination(transform.position);
                //TODO Add Gold to killer 
                //AddGoldToKillers();
                CurrentState = CreepState.Dead;
            }
            return;
        }
        if (CurrentWaypoint != null && Vector3.Distance(transform.position, CurrentWaypoint.GetPosition()) <= agent.stoppingDistance && CurrentWaypoint.nextWaypoint != null)
		{
            CurrentWaypoint = CurrentWaypoint.nextWaypoint;
		}

		CurrentState = GetCurrentState();
		ExecuteAction(CurrentState);
		FaceTarget();
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
                return allEnemyNear.Count == 0;
            case CreepStateAction.MoveToEnemyCreep:
                return CheckToMoveToEnemyByTag(allEnemyNear, "Creept");
            case CreepStateAction.MoveToEnemyHero:
                return CheckToMoveToEnemyByTag(allEnemyNear, "Player");
            case CreepStateAction.MoveToEnemyTower:
                return CheckToMoveToEnemyByTag(allEnemyNear, "Tower");
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
					agent.SetDestination(target.transform.position);
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
            if (item.tag.Contains(enemyTag))
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
    private bool CheckUnitIsNotNullAndAlive(TargetData unit)
    {
        return unit != null && unit.transform != null && unit.health != null && unit.health.isAlive;
    }
    #endregion
}
