using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepManager : SingletonBase<CreepManager>
{
    private int creepId = 0;
    private object creepIdLock = new object();

    public int GetNewCreepId()
    {
        int newId;
        lock (creepIdLock)
        {
            newId = creepId;
            creepId++;
        }
        return newId;
    }

}
