using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseCurrentHpSubAbility : HealSubAbilityBase
{
	public IncreaseCurrentHpSubAbilityData subAbilityData;
	public override void Init()
	{
		subAbilityData.healType = Dobeil.HealSubAbilityType.IncreaseCurrentHp;
	}

	public override void CastSubAbility()
	{
        if (subAbilityData.subAbilityNumber.Type == Dobeil.AdditionTypeEnum.FixedNumber)
			hero.health.Heal(Mathf.RoundToInt(subAbilityData.subAbilityNumber.GetCount()));
		else
			hero.health.Heal(Mathf.RoundToInt(subAbilityData.subAbilityNumber.GetCount() * hero.health.HpCount));
	}
	
}
