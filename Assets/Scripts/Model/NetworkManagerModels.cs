using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NetworkManagerModels
{
    public enum ClientToServerId : ushort
    {
        validation = 1
    }

    public enum ServerToClientId : ushort
    {
        validation = 1,
        LobbyIsReady = 2
    }
    [Serializable]
    public class ListWithChangeEvent<T> : List<T>
    {
        public event Action<T> ItemAdded;
        public event Action<T> ItemRemoved;

        public new void Add(T item)
        {
            base.Add(item);
            ItemAdded?.Invoke(item);
        }

        public new void Remove(T item)
		{
            Debug.LogError("Bingooo ");
            base.Remove(item);
            Debug.LogError(Count);
            ItemRemoved?.Invoke(item);
		}
    }

    //   public class ConnectedUserInQueue
    //{
    //       public float connectedTime;
    //       public ushort userId;
    //       public ConnectedUserInQueue() { }
    //       public ConnectedUserInQueue(float time, ushort id)
    //	{
    //           connectedTime = time;
    //           userId = id;
    //	}
    //}
}
