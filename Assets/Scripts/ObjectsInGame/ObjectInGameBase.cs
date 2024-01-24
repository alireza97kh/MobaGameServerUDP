using Dobeil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class ObjectInGameBase : MonoBehaviour
{
    public GameElement element;
    public Teams team;
    public string enemyTag;
    public Health health;
    public Collider Collider;
    public NavMeshAgent agent;

    protected virtual void Init(LobbyManager manager)
    {
        manager.objectsInGame.Add(Collider.GetInstanceID(), this);
    }

    protected void SetEnemyTag()
    {
        enemyTag = team == Teams.Team1 ? "Team2" : "Team1";
    }


}
