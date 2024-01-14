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
}