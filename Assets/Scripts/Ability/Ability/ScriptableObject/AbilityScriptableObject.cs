using Dobeil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "AbilityScriptableObject", menuName = "DobeilData/AbilityScriptableObject")]
public class AbilityScriptableObject : ScriptableObject
{
	public List<SavedAbilityClass> savedHerosPrefab = new List<SavedAbilityClass>();
	public Dictionary<AbilityLevelNameEnum, AbilityClass> heroAbility = new Dictionary<AbilityLevelNameEnum, AbilityClass>();
}
