using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "IncreaseMaxHpSubAbilityData", menuName = "SubAbility/IncreaseMaxHpSubAbilityData")]
public class IncreaseMaxHpSubAbilityData : HealSubAbilityBaseScriptableObject
{
    public IncreaseMaxHpSubAbilityData()
    {
        Type = Dobeil.SubAbilityType.Heal;
        healType = Dobeil.HealSubAbilityType.IncreaseMaxHp;
    }
}
