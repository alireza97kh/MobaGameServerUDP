using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dobeil;
public abstract class AbilityBase : MonoBehaviour
{
    public string abilityName;
    public Sprite abilityIcon;
    public List<SubAbilityBase> subAbilities = new List<SubAbilityBase>();

    public abstract void Init();
}
