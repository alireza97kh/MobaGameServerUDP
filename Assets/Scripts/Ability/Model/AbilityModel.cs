using Sirenix.OdinInspector;
using System;

namespace Dobeil
{
	public enum AbilitieType : ushort
	{
		Active = 0,
		Passive = 1,
		Autocast = 2
	}
	public enum LevelEnum : ushort
	{
		One = 1,
		Two = 2,
		Three = 3,
		Four = 4
	}
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
		[ShowIf("C_Percent")][PropertyRange(0, 1)] public float Percent = 0;
		[ShowIf("C_Fixed")] public bool IsRandomCount = false;
		[ShowIf("C_Fixed2")] public int Count = 0;
		[ShowIf("C_FixedRandom")] public RangeDataClass CountRange;

		bool C_Percent()
		{
			return Type == AdditionTypeEnum.Percent;
		}

		bool C_Fixed()
		{
			return Type == AdditionTypeEnum.FixedNumber;
		}

		bool C_FixedRandom()
		{
			return C_Fixed() && IsRandomCount;
		}
		bool C_Fixed2()
		{
			return C_Fixed() && !IsRandomCount;
		}
	}
}