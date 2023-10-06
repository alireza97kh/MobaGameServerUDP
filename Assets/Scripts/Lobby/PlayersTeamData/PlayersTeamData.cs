using UnityEngine;
namespace Dobeil
{
	[System.Serializable]
	[CreateAssetMenu(fileName = "PlayersTeamData", menuName = "DobeilData/PlayersTeamData")]
	public class PlayersTeamData : ScriptableObject
	{
		public Vector3 teamPlayerSpawnPosition;

	}
}