using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterControllerClass : MonoBehaviour
{
	public ushort pId;
	public LobbyManager manager;
	public string selectedHeroId = "";
	public string userId = "";
    public ushort characterId;
    public NavMeshAgent character;
    public float speed;

	void Move(float horizontal, float vertical)
	{
		Vector3 move = transform.forward * vertical + transform.right * horizontal;
		character.SetDestination(move * Time.deltaTime * speed);
	}

	public void OnSelectedHero(string heroId, string _userId, ushort _pId, LobbyManager _manager)
	{
		selectedHeroId = heroId;
		userId = _userId;
		pId = _pId;
		manager = _manager;
	}

	public void Init()
	{

	}
}
