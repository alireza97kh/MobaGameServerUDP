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
	public NumberRange creeptsAttackDamageRange;
	public float rotationSpeed = 1000;
	public float creeptDellay = 2;
	public CreepActionScores score;
	public float creepDetection = 7;
}
