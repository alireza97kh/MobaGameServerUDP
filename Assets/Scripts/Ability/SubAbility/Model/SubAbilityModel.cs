using Sirenix.OdinInspector;
using System;

namespace Dobeil
{
	public enum SubAbilityType : ushort
	{
		None = 0,
		Heal = 1,
		Sheild = 2,
		Damage = 3,
		Armor = 4,
		MagicResistance = 5,
		MovementSpeed = 6,
		AttackSpeed = 7,
		Silence = 8,
		Stun = 9,
		Slow = 10,
		MoveInDirection = 11,
		CreateWard = 12,
		StealDamage = 13,
		StealHelth = 14,
		HpSwap = 15,
		AntiHeal = 16,
		AntiAbility = 17,
		CreateShadow = 18,
		ReverseAttack = 19,
		Fear = 20,
		Anger = 21,
		Push = 22,
		Invisible = 23,
		AttackRandomEnemy = 24,
		SplitShot = 25,
		ChangeDimention = 26,
		ArmyOfCreep = 27,
		Transform = 28,
		Missed = 29
	}
	public enum AdditionTypeEnum
	{
		Percent = 0,
		FixedNumber = 1
	}

	[System.Serializable]
	public class RangeDataClass
	{
		public int Min;
		public int Max;

		public RangeDataClass()
		{
			Min = 0;
			Max = 0;
		}

		public RangeDataClass(int _min, int _max)
		{
			Min = _min;
			Max = _max;
		}
	}
	[Serializable]
	public class SubAbilityModificationData
	{
		[EnumToggleButtons] [HideLabel] public AdditionTypeEnum Type;
		[ShowIf("IsPercent")][PropertyRange(0, 1)] public float Percent = 0;
		[ShowIf("IsFixed")] public bool IsRandomCount = false;
		[ShowIf("IsFixedNumber")] public float Count = 0;
		[ShowIf("IsFixedRandom")] public RangeDataClass CountRange;

		private bool IsPercent()
		{
			return Type == AdditionTypeEnum.Percent;
		}

		private bool IsFixed()
		{
			return Type == AdditionTypeEnum.FixedNumber;
		}

		private bool IsFixedRandom()
		{
			return IsFixed() && IsRandomCount;
		}
		private bool IsFixedNumber()
		{
			return IsFixed() && !IsRandomCount;
		}

		public float GetCount()
		{
			if (IsFixed())
			{
				if (IsFixedNumber())
					return Count;
				else
					return UnityEngine.Random.Range(CountRange.Min, CountRange.Max);
			}
			else
				return Percent;
		}
	}
}