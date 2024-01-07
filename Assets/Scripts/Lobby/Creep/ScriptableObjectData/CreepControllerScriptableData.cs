using Dobeil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(fileName = "CreepControllerScriptableData", menuName = "DobeilData/CreepControllerScriptableData")]
public class CreepControllerScriptableData : ScriptableObject
{
	public float creeptAttackPlane = 3;
	public float creeptAround = 10;
	public int creepAttackDamage;
	public float rotationSpeed = 1000;
	public float creeptDecesionDellay = 0.1f;
	public float creeptAttackDellay = 0.5f;
	public CreepActionScores score;
	public float creepDetection = 7;
	public int sendSyncMessageTick = 15;
	public DamageType creepDamageType;
}
