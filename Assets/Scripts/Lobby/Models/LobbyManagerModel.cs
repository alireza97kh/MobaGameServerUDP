namespace Dobeil
{
	public enum LoadGameSteps : ushort
	{
		LoadGameScene = 0,
		LoadPlayers = 1,
		LoadTowers = 2,
		LoadCreeps = 3
	}
	public class UserInLobbyData
	{
		public ushort pId;
		public PlayerController player;
		public HeroControllerBase hero;
	}
}