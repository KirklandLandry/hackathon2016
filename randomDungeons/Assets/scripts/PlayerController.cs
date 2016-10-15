﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	private Vector3 velocity;
	private float acceleration = 50;
	private float friction = 0.3f;

	public Camera thisCamera;
	public GameObject model;

	void Start () 
	{
		// prevent physics based rotations
		transform.GetComponent<Rigidbody>().freezeRotation = true;
	}
	
	void Update () 
	{
		float dt = Time.deltaTime;

		if(tag == "player1")
		{
			//Vector3 movementDirection = new Vector3(0,0,0);
			if(Input.GetKey(KeyCode.A))
			{
				//movementDirection -= new Vector3(1, 0, 0);
				velocity.x -= (acceleration * Time.deltaTime);
			}
			else if(Input.GetKey(KeyCode.D))
			{
				//movementDirection += new Vector3(1, 0, 0);
				velocity.x += (acceleration * Time.deltaTime);
			}

			if(Input.GetKey(KeyCode.W))
			{
				//movementDirection += new Vector3(0, 0, 1);
				velocity.z += (acceleration * Time.deltaTime);
			}
			else if(Input.GetKey(KeyCode.S))
			{
				//movementDirection -= new Vector3(0, 0, 1);
				velocity.z -= (acceleration * Time.deltaTime);
			}

			Vector3 mousePos = Input.mousePosition;

			velocity.x -= (friction * velocity.x);
			velocity.z -= (friction * velocity.z);



			Vector2 centreToMouse;

			centreToMouse.x = mousePos.x - (Screen.width / 2);
			centreToMouse.y = mousePos.y - (Screen.height / 2);

			//transform.eulerAngles = new Vector3(0, Mathf.Atan2(centreToMouse.y, centreToMouse.x) * 180 / Mathf.PI, 0);


			//Get the Screen positions of the object
			Vector2 positionOnScreen = thisCamera.WorldToViewportPoint (transform.position);

			//Get the Screen position of the mouse
			Vector2 mouseOnScreen = (Vector2)thisCamera.ScreenToViewportPoint(Input.mousePosition);

			//Get the angle between the points
			float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);

			// 0 is left
			// top of screen is (-)
			// bottom of screen is (+)
			model.transform.rotation =  Quaternion.Euler (new Vector3(0f,-angle - 90, 0f));
			Debug.Assert(1==0, angle);

			transform.GetComponent<Rigidbody>().velocity = velocity;

		}


	}

	float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
		return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
	}


}
