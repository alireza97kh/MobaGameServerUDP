using NetworkManagerModels;
using Riptide;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGenerator : MonoBehaviour
{
    [SerializeField] private List<TowerController> towers = new List<TowerController>();
    private ushort towersId = 0;
    public void Init(string lobbyKey)
    {
        Message towerInitMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.CreateTower);
        towerInitMessage.AddUShort((ushort)towers.Count);
        foreach (var item in towers)
        {
            towersId++;
            item.Init(towersId);
            towerInitMessage.AddUShort(towersId);
            towerInitMessage.AddUShort((ushort)item.team);
            towerInitMessage.AddUShort((ushort)item.line);
            towerInitMessage = HelperMethods.Instance.AddVector3(item.transform.position, towerInitMessage);
            towerInitMessage = HelperMethods.Instance.AddShort(
                item.transform.rotation.eulerAngles.y, towerInitMessage);
        }
		NetworkManager.Instance.SendMessageToAllUsersInLobby(towerInitMessage, lobbyKey);
	}
}
