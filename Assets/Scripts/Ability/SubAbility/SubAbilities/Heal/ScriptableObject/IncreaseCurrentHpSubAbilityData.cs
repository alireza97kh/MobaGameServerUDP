using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "IncreaseCurrentHpSubAbilityData", menuName = "SubAbility/IncreaseCurrentHpSubAbilityData")]
public class IncreaseCurrentHpSubAbilityData : HealSubAbilityBaseScriptableObject
{
	public IncreaseCurrentHpSubAbilityData()
	{
		healType = Dobeil.HealSubAbilityType.IncreaseCurrentHp;
		Type = Dobeil.SubAbilityType.Heal;
	}
}
