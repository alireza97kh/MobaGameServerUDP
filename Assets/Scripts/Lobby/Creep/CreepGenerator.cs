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
	private int generatorId = -1;
	private string lobbyKey = "";
	public void Init(int _id, string _lobbyKey)
	{
		allOfCreatedCreep = new List<CreepController>();
		generatorId = _id;
		lobbyKey = _lobbyKey;
		StartCoroutine(GenerateCreep());
	}

	IEnumerator GenerateCreep()
	{
		while (true)
		{
			yield return new WaitForSeconds(generatorData.startDellay);
			while (true)
			{
				Message createNewCreepMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.CreateCreep);
				createNewCreepMessage.AddInt(generatorId);
				int Count = generatorData.countOfCreepts;
				createNewCreepMessage.AddInt(Count);
				createNewCreepMessage.AddVector3(firstWaypoint.GetPosition());
				for (int i = 1; i < allOfCreatedCreep.Count && Count > 0; i++)
				{
					if (!allOfCreatedCreep[i].isAlive && Count > 0)
					{
						allOfCreatedCreep[i].transform.position = transform.position;
						allOfCreatedCreep[i].firstWaypoint = firstWaypoint;
						allOfCreatedCreep[i].Init();
						createNewCreepMessage.AddInt((int)CreateCreepState.Restart);
						createNewCreepMessage.AddInt(allOfCreatedCreep[i].creepId);
						Count--;
					}
				}
				for (int i = 0; i < Count; i++)
				{
					CreepController newCreep = Instantiate(generatorData.creeptsPrefab, transform.position, Quaternion.identity, transform);
					newCreep.tag = generatorData.creepTag;
					newCreep.firstWaypoint = firstWaypoint;
					newCreep.creepId = CreepManager.Instance.GetNewCreepId();
					newCreep.Init(lobbyKey);
					createNewCreepMessage.AddInt((int)CreateCreepState.Instantiate);
					createNewCreepMessage.AddInt(newCreep.creepId);
					allOfCreatedCreep.Add(newCreep);
				}
				NetworkManager.Instance.SendMessageToAllUsersInLobby(createNewCreepMessage, lobbyKey);
				yield return new WaitForSeconds(generatorData.dellay);
			}
		}
	}

}
