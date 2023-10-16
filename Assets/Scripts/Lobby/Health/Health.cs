using Dobeil;
using NetworkManagerModels;
using Riptide;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int id;
    public string lobbyKey;
    public bool isAlive = true;
    public int hpCount = 0;


    public int maxHp = 0;

    public bool haveProtectionSheild = false;
    public int maxProtectionSheildCount = -1;

    public int increaseHpPerSec = -1;

    public int armor = 0; // max of these two is 100 so be aware of that 
    public int magicResistance = 0;

    private Dictionary<int, int> attackers;

    private int tick = 1;
    public int syncTick = 20;

    public void Init(string _lobbyKey, int _id)
	{
        lobbyKey = _lobbyKey;
        id = _id;
        isAlive = true;
        hpCount = maxHp;
        attackers = new Dictionary<int, int>();
		if (armor >= 100)
            armor = 100;
		if (magicResistance >= 100)
            magicResistance = 100;
	}

    public void DecreaseHp(int count, DamageType type, int attackerId, Action<bool> onKilledAction = null)
	{
        bool killedUnit = false;
        float percentOfActualDamage = type == DamageType.Physical ? armor / armor + 100 : magicResistance / magicResistance + 100;

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

    private bool UnitGetHit(int count, DamageType type, int attackerId, Action<bool> onKilledAction = null)
	{
        bool killedUnit = false;
        if (hpCount - count <= 0)
        {
            isAlive = false;
            hpCount = 0;
            killedUnit = true;
        }
        else
        {
            hpCount -= count;
            if (attackers.ContainsKey(attackerId))
                attackers[attackerId] += count;
            else
                attackers.Add(attackerId, count);
        }
        return killedUnit;
    }

    public void Heal(int count)
	{
		if (hpCount + count >= maxHp)
            hpCount = maxHp;
        else
            hpCount += count;
	}

	private void Update()
	{
        tick++;
		if (tick % syncTick == 0)
            SendSyncHealthMessage(); 
	}

    private void SendSyncHealthMessage()
	{
        Message healthMessage = Message.Create(MessageSendMode.Unreliable, ServerToClientId.SyncHealth);
        healthMessage.AddInt(id);
        healthMessage.AddInt(hpCount);
        healthMessage.AddInt(maxHp);
        NetworkManager.Instance.SendMessageToAllUsersInLobby(healthMessage, lobbyKey);
	}
}
