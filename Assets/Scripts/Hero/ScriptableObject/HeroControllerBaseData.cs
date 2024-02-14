using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "HeroControllerBaseData", menuName = "DobeilData/Hero/HeroControllerBaseData")]
public class HeroControllerBaseData : ObjectInGameBaseData
{
	public string heroName;
	public Sprite heroIcon;
	public float heroSpeed;
}
