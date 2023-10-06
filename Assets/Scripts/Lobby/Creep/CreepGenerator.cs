using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepGenerator : MonoBehaviour
{
	public CreepGeneratorData generatorData;
	[SerializeField] private Waypoint firstWaypoint;

	private List<CreepController> allOfCreatedCreep; // cash Created Creep
	public void Init()
	{
		allOfCreatedCreep = new List<CreepController>();
		StartCoroutine(GenerateCreep());
	}

	IEnumerator GenerateCreep()
	{
		while (true)
		{
			yield return new WaitForSeconds(generatorData.startDellay);
			while (true)
			{
				int Count = generatorData.countOfCreepts;
				for (int i = 1; i < allOfCreatedCreep.Count && Count > 0; i++)
				{
					if (!allOfCreatedCreep[i].isAlive && Count > 0)
					{
						allOfCreatedCreep[i].transform.position = transform.position;
						allOfCreatedCreep[i].firstWaypoint = firstWaypoint;
						allOfCreatedCreep[i].Init();
						Count--;
					}
				}
				for (int i = 0; i < Count; i++)
				{
					CreepController newCreep = Instantiate(generatorData.creeptsPrefab, transform.position, Quaternion.identity, transform);
					newCreep.tag = generatorData.creepTag;
					newCreep.firstWaypoint = firstWaypoint;
					newCreep.Init();
					allOfCreatedCreep.Add(newCreep);
				}
				yield return new WaitForSeconds(generatorData.dellay);
			}
		}
	}

}
