using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// a player's camera can be set to mask out the other players traps
// this makes them invisible to the enemy

public class PlayerController : MonoBehaviour {

	private Vector3 velocity;
	private float acceleration = 50;
	private float friction = 0.3f;

	public Camera thisCamera;
	public GameObject model;

	//private List<AnimationState> animationList;
	private List<string> animationNameList;

	void Start () 
	{
		// prevent physics based rotations
		transform.GetComponent<Rigidbody>().freezeRotation = true;
	
		animationNameList = new List<string>();
		int count = 0;



		foreach(AnimationState anim in model.transform.GetComponent<Animation>())
		{
			animationNameList.Add(anim.name);
			//print(animationNameList[count]);
			if (count >= 2)
			{
				anim.wrapMode = WrapMode.Once;
			}
			else 
			{
				anim.wrapMode = WrapMode.Loop;
			}
			count++;
		}

	}
	
	void Update () 
	{
		float dt = Time.deltaTime;

		if(tag == "player1")
		{


			velocity.x += Input.GetAxis("Horizontal") * acceleration * Time.deltaTime;
			velocity.z += Input.GetAxis("Vertical") * acceleration * Time.deltaTime;

			velocity.x -= (friction * velocity.x);
			velocity.z -= (friction * velocity.z);

			transform.GetComponent<Rigidbody>().velocity = velocity;

			float rightX = Input.GetAxis("RightHorizontal");
			float rightZ = -Input.GetAxis("RightVertical");

			if(Mathf.Sqrt(Mathf.Pow(rightX,2) + Mathf.Pow(rightZ, 2)) > 0.25)
			{

				float angle = Mathf.Atan2(rightZ, rightX) * 180 / Mathf.PI;
				//print(Mathf.Atan2(rightZ, rightX) * 180 / Mathf.PI);
				model.transform.rotation = Quaternion.Euler(new Vector3(0f, -angle + 90, 0f));
			}


			//float xPress = Input.GetAxisRaw("xButton");
			if(Input.GetButtonDown("xButton"))
			{
				print("x button");
			}

			if(Input.GetButtonDown("R1Button"))
			{
				print("R1 button");
			}

			if(Input.GetButtonDown("R2Button"))
			{
				print("R2 button");
			}

			if(Input.GetButtonDown("L1Button"))
			{
				print("L1 button");
			}

			if(Input.GetButtonDown("L2Button"))
			{
				print("L2 button");
			}





		}
		else 
		{
			velocity.x += Input.GetAxisRaw("KeyboardHorizontal") * acceleration * Time.deltaTime;
			velocity.z += Input.GetAxisRaw("KeyboardVertical") * acceleration * Time.deltaTime;

			velocity.x -= (friction * velocity.x);
			velocity.z -= (friction * velocity.z);

			transform.GetComponent<Rigidbody>().velocity = velocity;


			//Get the Screen positions of the object
			Vector2 positionOnScreen = thisCamera.WorldToViewportPoint (transform.position);
			//Get the Screen position of the mouse
			Vector2 mouseOnScreen = (Vector2)thisCamera.ScreenToViewportPoint(Input.mousePosition);
			//Get the angle between the points
			float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);
			model.transform.rotation =  Quaternion.Euler (new Vector3(0f,-angle - 90, 0f));


			if(Input.GetMouseButtonDown(0))
			{
				print("lmb");
			}

			if(Input.GetMouseButtonDown(1))
			{
				print("rmb");
			}

		}

		if( (Mathf.Abs(velocity.x) > 0.05f || Mathf.Abs(velocity.z) > 0.05f))
		{
			//print("go");
			model.GetComponent<Animation>().Play(animationNameList[1]);
		}
		else if(!model.GetComponent<Animation>().IsPlaying(animationNameList[0]))
		{
			//print("halt");
			model.GetComponent<Animation>().Play(animationNameList[0]);
		}

	}

	float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
		return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
	}


}
