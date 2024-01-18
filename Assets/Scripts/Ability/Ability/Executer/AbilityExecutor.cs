using Dobeil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityExecutor : MonoBehaviour
{
	public List<SavedAbilityClass> savedAbility = new List<SavedAbilityClass>();
	public Dictionary<AbilityLevelNameEnum, AbilityClass> heroAbilities = new Dictionary<AbilityLevelNameEnum, AbilityClass>();

	public void Awake()
	{
		foreach (var item in savedAbility)
			heroAbilities.Add(item.key, item.ability);
	}
}
