using Dobeil;
using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Collider), typeof(Health))]
public abstract class ObjectInGameBase : MonoBehaviour
{
    [HideInInspector] public ObjectInGameBaseData data;
	public GameElement element;
    public Teams team;
    public Health health;
    public Collider Collider;
    public NavMeshAgent agent;
    public bool isAlive = false;

    [HideInInspector] public ushort elementId;
	[HideInInspector] public string enemyTag;
	[HideInInspector] protected LobbyManager manager;
	[HideInInspector] public ObjectInGameBase target;
    public virtual void Init(LobbyManager _manager, ushort _elementId)
    {
		GetRequiredComponent();
		elementId = _elementId;
        manager = _manager;
        health.maxHp = data.maxHp;
		health.Init(manager.lobbyKey, elementId, element);
		tag = team.ToString() + element.ToString();
		manager.objectsInGame.Add(Collider.GetInstanceID(), this);
        isAlive = true;
        SetEnemyTag();
        gameObject.SetActive(true);
    }

    protected void SetEnemyTag()
    {
        enemyTag = team == Teams.Team1 ? "Team2" : "Team1";
    }

    protected void GetRequiredComponent()
    {
		if (agent == null)
			agent = GetComponent<NavMeshAgent>();
		if (health == null)
			health = GetComponent<Health>();
        if (Collider == null)
            Collider = GetComponent<Collider>();
	}

    protected List<ObjectInGameBase> FindAllTarget()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, data.detectionAround, data.attackLayer);
        List<ObjectInGameBase> enemyUnitsInRange = new List<ObjectInGameBase>();
        foreach (Collider item in col)
        {
            if (item.tag.Contains(enemyTag) && manager.objectsInGame.TryGetValue(item.GetInstanceID(), out ObjectInGameBase _target))
            {
                if (_target.isAlive)
                    enemyUnitsInRange.Add(_target);
            }
        }
        return enemyUnitsInRange;
	}

    protected bool CheckTargeIsAvaiable(ObjectInGameBase _target)
    {
        if (_target != null && _target.isAlive)
        {
			float distanceToTarget = Vector3.Distance(transform.position, _target.transform.position);
			return (distanceToTarget < data.attackRange);
		}
        return false;
    }

	protected ObjectInGameBase FindNearestEnemy(List<ObjectInGameBase> targets, string _tag = "")
	{
		if (targets.Count > 0)
		{
			int indexOfNearestEnemy = -1;
			float maxDist = data.attackRange;
			if (CheckTargeIsAvaiable(target))
				maxDist = Vector3.Distance(transform.position, target.transform.position);
			for (int i = 0; i < targets.Count; i++)
			{
				if (targets[i].tag.Contains(_tag) && Vector3.Distance(transform.position, targets[i].transform.position) < maxDist)
				{
					indexOfNearestEnemy = i;
				}
			}
			if (indexOfNearestEnemy >= 0 && indexOfNearestEnemy < targets.Count)
			{
				return targets[indexOfNearestEnemy];
			}
			else if (CheckTargeIsAvaiable(target))
			{
				return target;
			}
		}
		return null;
	}

	public void DecreaseHp(int count, DamageType type, string attackerId, Action<bool> onKilledAction = null)
    {
        health.DecreaseHp(count, type, attackerId, onKilledAction);
    }
}
