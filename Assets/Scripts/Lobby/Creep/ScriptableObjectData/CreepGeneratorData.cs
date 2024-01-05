using Dobeil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Dobeil
{
    public enum Lines : ushort
    {
        Top = 0,
        Mid = 1,
        Down = 2
    }

    public enum Teams : ushort
    {
        Team1 = 1,
        Team2 = 2
    }

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
    public Lines generatorLine;
    public Teams generatorTeam;
}
