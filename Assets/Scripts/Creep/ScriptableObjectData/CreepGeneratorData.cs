using Dobeil;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Dobeil
{
	public enum CreateCreepState : ushort
	{
        Instantiate = 0,
        Restart = 1
    }
}
[System.Serializable]
[CreateAssetMenu(fileName = "CreepGeneratorData", menuName = "DobeilData/CreepGeneratorData")]
public class CreepGeneratorData : ScriptableObject
{
    public CreepController creeptsPrefab;
    public float startDellay = 3;
    public float dellay;
    public ushort countOfCreepts = 3;
    public string creepTag = "";
	[EnumToggleButtons]
	public Lines generatorLine;
    [EnumToggleButtons]
    public Teams generatorTeam;
}
