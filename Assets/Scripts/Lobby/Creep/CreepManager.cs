using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepManager : SingletonBase<CreepManager>
{
    private ushort creepId = 1;
    private object creepIdLock = new object();

    public ushort GetNewCreepId()
    {
        ushort newId;
        lock (creepIdLock)
        {
            newId = creepId;
            creepId++;
        }
        return newId;
    }

}
