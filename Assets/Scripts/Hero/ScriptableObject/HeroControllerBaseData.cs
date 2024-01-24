using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "HeroControllerBaseData", menuName = "DobeilData/Hero/HeroControllerBaseData")]
public class HeroControllerBaseData : ScriptableObject
{
	public string heroName;
	public Sprite heroIcon;
	public float heroSpeed;
	public float heroAttackSpeed;
	public float heroAttackDellay;
	public float heroAttackArea = 10;
	public int heroBaseAttackDamage = 300;
	public int heroMaxHp;
}
