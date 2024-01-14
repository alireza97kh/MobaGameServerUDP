using Dobeil;
using NetworkManagerModels;
using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepGenerator : MonoBehaviour
{
	public CreepGeneratorData generatorData;
	[SerializeField] private Waypoint firstWaypoint;

	private List<CreepController> allOfCreatedCreep; // cash Created Creep
	private ushort generatorId;
	private string lobbyKey = "";
	private LobbyManager lobby;
	public void Init(ushort _id, string _lobbyKey, LobbyManager _lobby)
	{
		lobby = _lobby;
		allOfCreatedCreep = new List<CreepController>();
		generatorId = _id;
		lobbyKey = _lobbyKey;
		StartCoroutine(GenerateCreep());
	}

	IEnumerator GenerateCreep()
	{
		yield return new WaitUntil(() => lobby.gameStarted == false);
		while (true)
		{
			yield return new WaitForSeconds(generatorData.startDellay);
			while (true)
			{
				Message createNewCreepMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.CreateCreep);
				createNewCreepMessage.AddUShort(generatorId);
				ushort Count = generatorData.countOfCreepts;
				createNewCreepMessage.AddUShort(Count);
				createNewCreepMessage = HelperMethods.Instance.AddVector3(firstWaypoint.GetPosition(), createNewCreepMessage);
				for (int i = 1; i < allOfCreatedCreep.Count && Count > 0; i++)
				{
					if (!allOfCreatedCreep[i].isAlive && Count > 0)
					{
						allOfCreatedCreep[i].transform.position = transform.position;
						allOfCreatedCreep[i].firstWaypoint = firstWaypoint;
						allOfCreatedCreep[i].Init();
						createNewCreepMessage.AddUShort((ushort)CreateCreepState.Restart);
						createNewCreepMessage.AddUShort(allOfCreatedCreep[i].creepId);
						Count--;
					}
				}
				for (int i = 0; i < Count; i++)
				{
					CreepController newCreep = Instantiate(generatorData.creeptsPrefab, transform.position, Quaternion.identity, transform);
					newCreep.tag = generatorData.creepTag;
					newCreep.firstWaypoint = firstWaypoint;
					newCreep.Init(lobbyKey, CreepManager.Instance.GetNewCreepId());
					createNewCreepMessage.AddUShort((ushort)CreateCreepState.Instantiate);
					createNewCreepMessage.AddUShort(newCreep.creepId);
					allOfCreatedCreep.Add(newCreep);
				}
				NetworkManager.Instance.SendMessageToAllUsersInLobby(createNewCreepMessage, lobbyKey);
				yield return new WaitForSeconds(generatorData.dellay);
			}
		}
	}

}
