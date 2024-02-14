using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInGameBaseData : ScriptableObject
{
    public float attackRange;
    public float detectionAround;
    public int baseAttackDamage;
    public float attackDellay;
    public int sendSyncTick;
    public LayerMask attackLayer;
    public int maxHp;
}
