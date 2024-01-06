using Dobeil;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
	[ReadOnly] public ushort id;
	[EnumToggleButtons] [HideLabel] public TowersTeam team;
	[EnumToggleButtons] [HideLabel] public TowersLine line;
	public void Init(ushort _id)
	{
		id = _id;
	}
}
