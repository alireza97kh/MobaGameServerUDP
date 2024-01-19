using Sirenix.OdinInspector;
using System;

namespace Dobeil
{
	[EnumPaging]
	public enum HeroId
	{
		Dobeil = 0
	}
	[Serializable]
	public class HeroManagerClass
	{
		public HeroId Id;
		public HeroControllerBase hero;
	}
	public enum HeroInputType
	{
		Movement = 0,
		BaseAttack = 1,
		Ability = 2,
		Item = 3
	}
}