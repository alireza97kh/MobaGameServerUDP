using Dobeil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TowerActionScore", menuName = "DobeilData/TowerActionScore")]
public class TowerActionScore : ScriptableObject
{
    public TowerScore Idle;
    public TowerScore Attacking;
    public TowerScore Dead;
    public TowerActionScore()
    {
        Idle = new TowerScore();
        Attacking = new TowerScore();
        Dead = new TowerScore();
    }
}
