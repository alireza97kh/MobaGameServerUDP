using Dobeil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Dobeil
{
    public enum Lines
    {
        Top = 0,
        Mid = 1,
        Down = 2
    }

    public enum Teams
    {
        Team1 = 1,
        Team2 = 2
    }
}
[System.Serializable]
[CreateAssetMenu(fileName = "CreepGeneratorData", menuName = "DobeilData/CreepGeneratorData")]
public class CreepGeneratorData : ScriptableObject
{
    public CreepController creeptsPrefab;
    public float startDellay = 3;
    public float dellay;
    public int countOfCreepts = 3;
    public string creepTag = "";
    public Lines generatorLine;
    public Teams generatorTeam;
}
