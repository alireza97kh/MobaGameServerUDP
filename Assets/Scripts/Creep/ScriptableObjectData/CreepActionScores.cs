using Dobeil;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "CreepActionScores", menuName = "DobeilData/CreepActionScores")]
public class CreepActionScores : ScriptableObject
{
    public CreepScore Idle;
    public CreepScore Moving;
    public CreepScore MoveToTarget;
    public CreepScore Attacking;
    public CreepScore Dead;

    public CreepActionScores()
    {
        Idle = new CreepScore();
        Moving = new CreepScore();
		MoveToTarget = new CreepScore();
        Attacking = new CreepScore();
        Dead = new CreepScore();
    }


}
