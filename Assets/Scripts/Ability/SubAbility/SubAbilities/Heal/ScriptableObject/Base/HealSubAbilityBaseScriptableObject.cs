using Dobeil;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealSubAbilityBaseScriptableObject : SubAbilityBaseData
{
	[EnumToggleButtons]
	[HideLabel]
	public HealSubAbilityType healType;
}
