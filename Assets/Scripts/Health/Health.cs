using Dobeil;
using NetworkManagerModels;
using Riptide;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
	[HideInInspector] public ushort id;
    [EnumToggleButtons]
    public GameElement unitType;
	[HideInInspector] public string lobbyKey;
    public bool isAlive = true;
    public int HpCount { 
        get { return _hpCount; }
        set 
        {
            _hpCount = value;
            SendSyncHealthMessage();
        }
    }
    private int _hpCount;
    public int maxHp;
    public bool haveProtectionSheild = false;
    public int maxProtectionSheildCount = -1;

    public int increaseHpPerSec = -1;

    public int armor = 0;
    public int magicResistance = 0;

    private Dictionary<string, int> attackers;
    public void Init(string _lobbyKey, ushort _id, GameElement _unitType)
	{
        unitType = _unitType;
        lobbyKey = _lobbyKey;
        id = _id;
        isAlive = true;
        HpCount = maxHp;
        attackers = new Dictionary<string, int>();
	}

    public void DecreaseHp(int count, DamageType type, string attackerId, Action<bool> onKilledAction = null)
	{
        bool killedUnit = false;
        float percentOfActualDamage = (type == DamageType.Physical) ? armor / (armor + 100) : magicResistance / (magicResistance + 100);

        int numberOfResist = (int)(count * percentOfActualDamage);
        count -= numberOfResist;

        if (haveProtectionSheild)
		{
            if (maxProtectionSheildCount != -1)
            {
                if (maxProtectionSheildCount - count >= 0)
                    maxProtectionSheildCount -= count;
                else
                {
                    count -= maxProtectionSheildCount;
                    maxProtectionSheildCount = 0;
                    killedUnit = UnitGetHit(count, type, attackerId, onKilledAction);
                }
            }
		}
        else
		{
            killedUnit = UnitGetHit(count, type, attackerId, onKilledAction);
        }
        onKilledAction?.Invoke(killedUnit);

    }

    private bool UnitGetHit(int count, DamageType type, string attackerId, Action<bool> onKilledAction = null)
	{
        bool killedUnit = false;
        if (HpCount - count <= 0)
        {
            isAlive = false;
            HpCount = 0;
            killedUnit = true;
        }
        else
        {
            HpCount -= count;
            if (attackers.ContainsKey(attackerId))
                attackers[attackerId] += count;
            else
                attackers.Add(attackerId, count);
        }
        return killedUnit;
    }

    public void Heal(int count)
	{
		if (HpCount + count >= maxHp)
            HpCount = maxHp;
        else
            HpCount += count;
	}

    public void ChangeMaxHp(int count)
    {
        if (maxHp - count > 0)
        {
            maxHp -= count;
            if (HpCount > maxHp)
                HpCount = maxHp;
        }
    }

    private void SendSyncHealthMessage()
	{
        Message healthMessage = Message.Create(MessageSendMode.Unreliable, ServerToClientId.SyncHealth);
        healthMessage.AddUShort(id);
        healthMessage.AddUShort((ushort)unitType);
        healthMessage.AddInt(HpCount);
        healthMessage.AddInt(maxHp);
        NetworkManager.Instance.SendMessageToAllUsersInLobby(healthMessage, lobbyKey);
	}
}
