using Dobeil;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "TowerControllerScriptableObject", menuName = "DobeilData/TowerControllerScriptableObject")]
public class TowerControllerScriptableObject : ObjectInGameBaseData
{
    public float towerDecesionDellay = 0.1f;
    public TowerActionScore towerActionScore;
}
