using Dobeil;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubAbilityBaseData : ScriptableObject
{
	[EnumPaging]
	public string subAbilityName;
	public SubAbilityType Type;
	public SubAbilityModificationData subAbilityNumber;
	public bool requiresTarget;
	[ShowIf("requiresTarget")] public LayerMask targetLayer;
	public float castTime = 0;
	public float manaCost = 0;
}
