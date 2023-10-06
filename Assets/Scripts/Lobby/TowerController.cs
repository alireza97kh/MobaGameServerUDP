using Dobeil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : DobeilAiCharacterBase
{
	public override bool isActionAvaiable(AiState currentState, AiAction nextAction)
	{

		switch (currentState.stateName)
		{
			
			default:
				break;
		}

		return true;
	}

	bool IsEnemyAround()
	{
		return true;
	}

}
