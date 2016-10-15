using UnityEngine;
using System.Collections;

public class SpawnBeaconController : MonoBehaviour {

	public ParticleSystem spawnEffect;

	string targetTag;// = "player2";

	bool containsPlayer1;
	bool containsPlayer2;

	public void SetTargetTag(string target)
	{
		this.targetTag = target;
	}

	public ParticleSystem SpawnEffect(Vector3 position)
	{
		return (ParticleSystem)Instantiate(spawnEffect, position, Quaternion.identity);
	}

	public bool WinConditionSatisfied()
	{
		if(targetTag == "player1" && containsPlayer1 && !containsPlayer2)
		{
			return true;
		}
		else if(targetTag == "player2" && containsPlayer2 && !containsPlayer1)
		{
			return true;
		}
		else 
		{
			return false;
		}
	}

	public void OnTriggerEnterChild(string colliderTag)
	{
		if(colliderTag == "player1")
		{
			containsPlayer1 = true;
		}
		else 
		{
			containsPlayer2 = true;
		}
		print(colliderTag + " enter" + (targetTag == "player1" ? " p2 spawn" : "p1 spawn"));
	}


	public void OnTriggerExitChild(string colliderTag)
	{
		if(colliderTag == "player1")
		{
			containsPlayer1 = false;
		}
		else 
		{
			containsPlayer2 = false;
		}
		print(colliderTag + " exit" + (targetTag == "player1" ? " p2 spawn" : "p1 spawn"));
	}

}
