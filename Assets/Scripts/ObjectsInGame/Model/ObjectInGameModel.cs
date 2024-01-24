using Sirenix.OdinInspector;

namespace Dobeil
{
	[EnumToggleButtons()]
	public enum GameElement
	{
		None = 0,
		Hero = 1,
		Creep = 2,
		Structure = 3,
		NeutralMonsters = 4,
		Bosses = 5,
		Wards = 6,
		Buildings = 7
	}

	public enum Lines : ushort
	{
		Top = 0,
		Mid = 1,
		Down = 2
	}

	public enum Teams : ushort
	{
		Team1 = 0,
		Team2 = 1
	}
}