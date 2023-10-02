using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterControllerClass : MonoBehaviour
{
	public string selectedHeroId = "";
	public string userId = "";
    public ushort characterId;
    public NavMeshAgent character;
    public float speed;

	private void Awake()
	{
		character.Warp(transform.position);
	}

	private void Update()
	{
		Move();
	}
	void Move()
	{
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		Vector3 move = transform.forward * vertical + transform.right * horizontal;
		character.Move(move * Time.deltaTime * speed);
	}

	public void OnSelectedHero(string heroId, string _userId)
	{
		selectedHeroId = heroId;
		userId = _userId;
	}
}
