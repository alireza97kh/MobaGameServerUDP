using Dobeil;
using NetworkManagerModels;
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
				int count = generatorData.countOfCreepts;
				List<string> createNewCreepMessage = new List<string>()
				{
					((int)ServerToClientId.CreateCreep).ToString(),
					generatorId.ToString(),
                    count.ToString(),
                    firstWaypoint.GetPosition().ToString(),
                };

                for (int i = 1; i < allOfCreatedCreep.Count && generatorData.countOfCreepts > 0; i++)
				{
					if (!allOfCreatedCreep[i].isAlive && count > 0)
					{
						allOfCreatedCreep[i].transform.position = transform.position;
						allOfCreatedCreep[i].firstWaypoint = firstWaypoint;
						allOfCreatedCreep[i].Init();
						createNewCreepMessage.Add(((int)CreateCreepState.Restart).ToString());
						createNewCreepMessage.Add(allOfCreatedCreep[i].creepId.ToString());
						count--;
					}
				}
				for (int i = 0; i < count; i++)
				{
					CreepController newCreep = Instantiate(generatorData.creeptsPrefab, transform.position, Quaternion.identity, transform);
					newCreep.tag = generatorData.creepTag;
					newCreep.firstWaypoint = firstWaypoint;
					newCreep.Init(lobbyKey, CreepManager.Instance.GetNewCreepId());
					createNewCreepMessage.Add(((int)CreateCreepState.Instantiate).ToString());
					createNewCreepMessage.Add(newCreep.creepId);
					allOfCreatedCreep.Add(newCreep);
				}
				OnlineServer.Instance.BroadCastMessageInLobby(
					Utility.EnCodeMessage(createNewCreepMessage), 
					lobbyKey,
					SendMessageProtocol.TCP);
				yield return new WaitForSeconds(generatorData.dellay);
			}
		}
	}

}
