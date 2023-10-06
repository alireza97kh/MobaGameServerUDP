using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dobeil
{
	[System.Serializable]
	[CreateAssetMenu(fileName = "DobeilAiCharacterBaseDataModel", menuName = "DobeilData/DobeilAiCharacterBaseDataModel")]

	public class DobeilAiCharacterBaseDataModel : SerializedScriptableObject
	{
		[ShowInInspector] public Dictionary<string, AiState> allStates;
		public DobeilAiCharacterBaseDataModel()
		{
			allStates = new Dictionary<string, AiState>();
		}
	}
	[Serializable]
	public class AiState
	{
		[ShowInInspector] public string stateName;
		[ShowInInspector] public List<AiAction> stateActions;
		[ShowInInspector] public Action executeState;
	}
	[Serializable]
	public class AiAction
	{
		[ShowInInspector] public string actionName;
		[ShowInInspector] public string nextStateName;
		[ShowInInspector] public int actionScore;
	}
}