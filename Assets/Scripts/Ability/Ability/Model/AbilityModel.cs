using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dobeil
{
	[Serializable]
	public class AbilityClass
	{
		public string abilityName;
		public Sprite abilitySprite;
		[EnumToggleButtons]
		public AbilityType type;
		public float baseManaCost;
		[ShowIf("type", AbilityType.Active)] public float baseCastTime;
		[ShowIf("type", AbilityType.Active)] public float coolDown;

		public List<SubAbilityBase> subAbilities = new List<SubAbilityBase>();
	}

	[Serializable]
	public class SavedAbilityClass
	{
		public AbilityLevelNameEnum key;
		public AbilityClass ability;
	}

	public enum AbilityType : ushort
	{
		Active = 0,
		Passive = 1,
		Autocast = 2
	}
	[EnumToggleButtons]
	[HideLabel]
	public enum AbilityLevelNameEnum : ushort
	{
		One = 1,
		Two = 2,
		Three = 3,
		Four = 4
	}
}