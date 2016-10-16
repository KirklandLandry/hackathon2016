using UnityEngine;
using System.Collections;

public class PlayerWeaponCollision : MonoBehaviour 
{
	/*public bool containsPlayer1;
	public Rigidbody p1RigidBody;
	public bool containsPlayer2;
	public Rigidbody p2RigidBody;*/

	/*string myTag;

	public void SetEnemyTag(string myTag)
	{
		this.myTag = myTag;
	}*/


	bool attackActive = false;
	public void SetAttackActive(bool s)
	{
		attackActive = s;
	}

	void OnTriggerEnter(Collider collider)
	{
		if(attackActive)
		{
			if(collider.transform.tag == "player1")
			{
				print("hit p1");
				collider.transform.GetComponent<PlayerController>().hit();
				//collider.transform.GetComponent<Rigidbody>().AddForce(Vector3.right * 100);
				//containsPlayer1 = true;
				//p1RigidBody = collider.transform.GetComponent<Rigidbody>();
			}
			else if(collider.transform.tag == "player2")
			{
				print("hit p2");
				collider.transform.GetComponent<PlayerController>().hit();
				//p2RigidBody = collider.transform.GetComponent<Rigidbody>();
				//containsPlayer2 = true;
			}
		}

	}

	/*
	void OnTriggerExit(Collider collider)
	{
		if(collider.transform.tag == "player1")
		{
			//containsPlayer1 = false;
		}
		else if(collider.transform.tag == "player2")
		{
			//containsPlayer2 = false;
		}
	}*/
}
