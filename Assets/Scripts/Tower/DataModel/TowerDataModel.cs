namespace Dobeil
{
	public enum TowersTeam : ushort
	{
		Team1 = 0,
		Team2 = 1
	}

	public enum TowersLine : ushort
	{
		Top = 0,
		Middle = 1,
		Down = 2
	}
	public enum TowerState : ushort
	{
		Idle = 0,
		Attacking = 1,
		Dead = 2
	}

	public enum TowerStateAction
	{
		DoNothing,
		AttackToEnemyCreep,
		AttackToEnemyHero,
	}

	[System.Serializable]
	public class TowerScore
	{
		public float DoNothing;
		public float AttackToEnemyCreep;
		public float AttackToEnemyHero;
	}
}