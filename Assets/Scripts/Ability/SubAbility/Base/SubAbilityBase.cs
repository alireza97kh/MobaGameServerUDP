using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dobeil;
using Sirenix.OdinInspector;
public abstract class SubAbilityBase : MonoBehaviour
{
	[Required] public HeroControllerBase hero;
	public abstract void Init();

	public abstract void CastSubAbility();

}
