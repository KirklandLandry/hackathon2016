using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// a player's camera can be set to mask out the other players traps
// this makes them invisible to the enemy

public class PlayerController : MonoBehaviour {

	enum STATE
	{
		wait,
		walk,
		attack,
		damage,
		die
	}

	private float respawnTimer;

	private STATE currentState;

	private Vector3 velocity;
	private float acceleration = 50;
	private float friction = 0.3f;

	public Camera thisCamera;
	public GameObject model;
	public GameObject weapon;

	//private List<AnimationState> animationList;
	private List<string> animationNameList;

	public Vector3 startPos;

    private bool controlEnabled;

	void Start () 
	{
        controlEnabled = true;
        // prevent physics based rotations
        transform.GetComponent<Rigidbody>().freezeRotation = true;
	
		animationNameList = new List<string>();
		int count = 0;

		currentState = STATE.wait;

		//weapon.GetComponent<PlayerWeaponCollision>().SetEnemyTag(tag);

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

    public void DisableControl()
    {
        controlEnabled = false;
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        if (!currentState.Equals(STATE.die))
        {
            model.GetComponent<Animation>().Play(animationNameList[0]);
        }
    }

    public void EnableControl()
    {
        controlEnabled = true;
        model.GetComponent<Animation>().Play(animationNameList[0]);
    }
	
	void Update () 
	{
        if(controlEnabled)
        {
            float dt = Time.deltaTime;

            if (tag == "player1")
            {

                if (WaitingOrWalking())
                {
                    velocity.x += Input.GetAxis("Horizontal") * acceleration * Time.deltaTime;
                    velocity.z += Input.GetAxis("Vertical") * acceleration * Time.deltaTime;



                    float rightX = Input.GetAxis("RightHorizontal");
                    float rightZ = -Input.GetAxis("RightVertical");

                    if (Mathf.Sqrt(Mathf.Pow(rightX, 2) + Mathf.Pow(rightZ, 2)) > 0.25)
                    {

                        float angle = Mathf.Atan2(rightZ, rightX) * 180 / Mathf.PI;
                        //print(Mathf.Atan2(rightZ, rightX) * 180 / Mathf.PI);
                        model.transform.rotation = Quaternion.Euler(new Vector3(0f, -angle + 90, 0f));
                    }

                }

                velocity.x -= (friction * velocity.x);
                velocity.z -= (friction * velocity.z);

                transform.GetComponent<Rigidbody>().velocity = velocity;


                //float xPress = Input.GetAxisRaw("xButton");
                if (Input.GetButtonDown("xButton"))
                {
                    //print("x button");
                }

                if (Input.GetButtonDown("R1Button"))
                {
                    //print("R1 button");
                    if (WaitingOrWalking())
                    {
                        currentState = STATE.attack;
                        model.GetComponent<Animation>().Play(animationNameList[2]);
                        weapon.GetComponent<PlayerWeaponCollision>().SetAttackActive(true);
                    }
                }

                if (Input.GetButtonDown("R2Button"))
                {
                    //print("R2 button");
                }

                if (Input.GetButtonDown("L1Button"))
                {
                    //print("L1 button");
                }

                if (Input.GetButtonDown("L2Button"))
                {
                    //print("L2 button");
                }



            }
            else
            {
                if (WaitingOrWalking())
                {
                    velocity.x += Input.GetAxisRaw("KeyboardHorizontal") * acceleration * Time.deltaTime;
                    velocity.z += Input.GetAxisRaw("KeyboardVertical") * acceleration * Time.deltaTime;




                    //Get the Screen positions of the object
                    Vector2 positionOnScreen = thisCamera.WorldToViewportPoint(transform.position);
                    //Get the Screen position of the mouse
                    Vector2 mouseOnScreen = (Vector2)thisCamera.ScreenToViewportPoint(Input.mousePosition);
                    //Get the angle between the points
                    float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);
                    model.transform.rotation = Quaternion.Euler(new Vector3(0f, -angle - 90, 0f));


                }

                velocity.x -= (friction * velocity.x);
                velocity.z -= (friction * velocity.z);

                transform.GetComponent<Rigidbody>().velocity = velocity;

                if (Input.GetMouseButtonDown(0))
                {
                    //print("lmb");

                    if (WaitingOrWalking())
                    {
                        currentState = STATE.attack;
                        model.GetComponent<Animation>().Play(animationNameList[2]);
                        weapon.GetComponent<PlayerWeaponCollision>().SetAttackActive(true);

                        /*if(weapon.GetComponent<PlayerWeaponCollision>().containsPlayer1)
                        {
                        }*/
                    }
                }

                if (Input.GetMouseButtonDown(1))
                {
                    //print("rmb");
                }

            }

            if (currentState.Equals(STATE.die))
            {
                if (!model.GetComponent<Animation>().IsPlaying(animationNameList[4]))
                {
                    //currentState = STATE.wait;
                }
                respawnTimer -= Time.deltaTime;
                if (respawnTimer <= 0)
                {
                    transform.position = startPos;
                    currentState = STATE.wait;
                }
            }
            else if (currentState.Equals(STATE.attack))
            {
                if (!model.GetComponent<Animation>().IsPlaying(animationNameList[2]))
                {
                    weapon.GetComponent<PlayerWeaponCollision>().SetAttackActive(false);
                    currentState = STATE.wait;
                }

            }
            else
            {
                if ((Mathf.Abs(velocity.x) > 0.05f || Mathf.Abs(velocity.z) > 0.05f))
                {
                    //print("walking");
                    model.GetComponent<Animation>().Play(animationNameList[1]);
                }
                else if (!model.GetComponent<Animation>().IsPlaying(animationNameList[0]))
                {
                    //print("neutral");
                    model.GetComponent<Animation>().Play(animationNameList[0]);
                }
            }
        }
	
	}

	public void hit()
	{
		if(!currentState.Equals(STATE.die))
		{
			currentState = STATE.die;
			model.GetComponent<Animation>().Play(animationNameList[4]);
			respawnTimer = 3.0f;
		}
	}

	private bool WaitingOrWalking()
	{
		return currentState.Equals(STATE.wait) || currentState.Equals(STATE.walk);
	}

	float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
		return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
	}


}
