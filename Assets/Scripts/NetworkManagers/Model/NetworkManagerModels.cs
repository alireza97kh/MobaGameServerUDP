using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NetworkManagerModels
{
    public enum ClientToServerId : ushort
    {
        Validation,
        SelectedHero,
        LoadGame,
        CharacterInput,
        Ping
    }



    public enum ServerToClientId : ushort
    {
        validation,
        LobbyIsReady,
        HeroSelected,
        AllHeroSelected,
        CreateHero,
        StartGame,
        Pong,
        CreateCreepGenerator,
        CreateCreep,
        SyncCreep,
        SyncHealth,
        Sync,
        CreateTower,
        TowerShoot
    }


    [Serializable]
    public class ListWithEvent<T> : List<T>
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
            base.Remove(item);
            ItemRemoved?.Invoke(item);
		}
    }

    [Serializable]
    public class DictionaryWithEvent<T1, T2> : Dictionary<T1, T2>
	{
        public event Action<T1, T2> ItemAdded;
        public event Action<T1> ItemRemoved;

        public new void Add(T1 key, T2 value)
		{
            base.Add(key, value);
			ItemAdded?.Invoke(key, value);
		}

        public new void Remove(T1 key)
		{
            base.Remove(key);
            ItemRemoved?.Invoke(key);
		}

	}

    [Serializable]
    public class UsersHeroInLobby
	{
        public string userId;
        public string heroId;
        public UsersHeroInLobby() { }
        public UsersHeroInLobby(string _userId, string _heroId)
		{
            userId = _userId;
            heroId = _heroId;
		}
	}

    [Serializable]
    public class UsersHeroInLobbyList
    {
        public List<UsersHeroInLobby> usersData;
        public UsersHeroInLobbyList()
		{
            usersData = new List<UsersHeroInLobby>();
		}
        public UsersHeroInLobbyList(List<UsersHeroInLobby> _list)
		{
            usersData = new List<UsersHeroInLobby>();
			foreach (var item in _list)
			{
                usersData.Add(item);
			}
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
