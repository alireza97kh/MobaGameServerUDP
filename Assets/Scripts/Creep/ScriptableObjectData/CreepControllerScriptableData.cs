using Dobeil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(fileName = "CreepControllerScriptableData", menuName = "DobeilData/CreepControllerScriptableData")]
public class CreepControllerScriptableData : ObjectInGameBaseData
{
	public float creeptDecesionDellay = 0.1f;
	public CreepActionScores score;
}
