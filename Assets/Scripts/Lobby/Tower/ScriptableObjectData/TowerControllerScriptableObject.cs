using Dobeil;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "TowerControllerScriptableObject", menuName = "DobeilData/TowerControllerScriptableObject")]
public class TowerControllerScriptableObject : ScriptableObject
{
    public float towerAround;
    public int towerAttackDamage;
    public float towerDecesionDellay = 0.1f;
    public float towerAttackDellay = 0.5f;
    public int sendSyncMessageTick = 15;
    [EnumToggleButtons]
    public DamageType towerDamageType;
    public TowerActionScore towerActionScore;
}
