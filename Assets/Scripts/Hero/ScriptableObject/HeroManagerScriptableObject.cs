using Dobeil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
[CreateAssetMenu(fileName = "HeroManagerScriptableObject", menuName = "DobeilData/HeroManagerScriptableObject")]
public class HeroManagerScriptableObject : ScriptableObject
{
    public List<HeroManagerClass> savedHerosPrefab = new List<HeroManagerClass>();//  this list is just for save hero data in inspector

    public Dictionary<HeroId, HeroControllerBase> herosPrefab = new Dictionary<HeroId, HeroControllerBase>();
}
