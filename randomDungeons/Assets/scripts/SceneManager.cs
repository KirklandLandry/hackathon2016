using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// SPAWN BEACON RULES 
// can't get hit if you're int a spawn beacon
// also can't advance it the other person is in the spawn beacon

public class SceneManager : MonoBehaviour {

	MapGenerator mapGen;
	Map currentMap;
	// need to make map generation a co-routine so I can create the next map while playing current map
	// this should minimize load times to almost nothing after the initial load

	public GameObject floorTile;
	public GameObject wall;
	public GameObject spawnBeacon;
	public GameObject p1Avatar;
	public GameObject p2Avatar;
	public GameObject p1WinMessage;
	public GameObject p2WinMessage;

	private List<GameObject> currentSceneObjects;
	private List<ParticleSystem> activeParticleSystems;

	GameObject p1Beacon;
	GameObject p2Beacon;

	bool gameActive = false;

	// Use this for initialization
	void Start () 
	{
		currentSceneObjects = new List<GameObject>();
		activeParticleSystems = new List<ParticleSystem>();

		mapGen = new MapGenerator();
		currentMap = mapGen.NewMap(30, true, 44);
		PlaceEnvironment();

		p1WinMessage.SetActive(false);
		p2WinMessage.SetActive(false);
	}



	// Update is called once per frame
	void Update () 
	{
		if(gameActive)
		{
			if(p1Beacon.GetComponent<SpawnBeaconController>().WinConditionSatisfied())
			{
				print("p2 wins");
				p2WinMessage.SetActive(true);
			}
			else if (p2Beacon.GetComponent<SpawnBeaconController>().WinConditionSatisfied())
			{
				print("p1 wins");
				p1WinMessage.SetActive(true);
			}
		}
		if(Input.GetKeyDown(KeyCode.G))
		{
			TearDownEnvironment();
			currentMap = mapGen.NewMap(40, true, 44);
			PlaceEnvironment();
		}

		for(int i = activeParticleSystems.Count - 1; i >= 0; i--)
		{
			if(!activeParticleSystems[i].IsAlive())
			{
				Destroy(activeParticleSystems[i]);
				activeParticleSystems.RemoveAt(i);
			}
		}

	}

	void PlaceEnvironment()
	{
		gameActive = false;
		GameObject parent = new GameObject();
		parent.transform.position = new Vector3(0,0,0);

		currentSceneObjects.Add(parent);
		for(int y = 0; y < currentMap.Height; y++)
		{
			for(int x = 0; x < currentMap.Width; x++)
			{
				if(currentMap.TileAt(x, y) == currentMap.EmptyCode)
				{
					currentSceneObjects.Add((GameObject)GameObject.Instantiate(floorTile, new Vector3(x, 0, y), Quaternion.identity, parent.transform));
				}
				else 
				{
					currentSceneObjects.Add((GameObject)GameObject.Instantiate(wall, new Vector3(x, 0, y), Quaternion.identity, parent.transform));
				}
			}
		}

		p1Avatar.transform.position = new Vector3(currentMap.StartPos1.x, 0, currentMap.StartPos1.y);
		p1Avatar.transform.GetComponent<PlayerController>().startPos = p1Avatar.transform.position;
		p2Avatar.transform.position = new Vector3(currentMap.StartPos2.x, 0, currentMap.StartPos2.y);
		p2Avatar.transform.GetComponent<PlayerController>().startPos = p2Avatar.transform.position;

		p1Beacon = (GameObject)GameObject.Instantiate(spawnBeacon, new Vector3(currentMap.StartPos1.x, 0, currentMap.StartPos1.y), Quaternion.identity, parent.transform);
		currentSceneObjects.Add(p1Beacon);
		p1Beacon.GetComponent<SpawnBeaconController>().SetTargetTag("player2");

		activeParticleSystems.Add(p1Beacon.GetComponent<SpawnBeaconController>().SpawnEffect(p1Beacon.transform.position));

		p2Beacon = (GameObject)GameObject.Instantiate(spawnBeacon, new Vector3(currentMap.StartPos2.x, 0, currentMap.StartPos2.y), Quaternion.identity, parent.transform);
		currentSceneObjects.Add(p2Beacon);
		p2Beacon.GetComponent<SpawnBeaconController>().SetTargetTag("player1");
		p2Beacon.GetComponent<SpawnBeaconController>().SpawnEffect(p2Beacon.transform.position);
		gameActive = true;
	}

	void TearDownEnvironment()
	{
		p1WinMessage.SetActive(false);
		p2WinMessage.SetActive(false);

		for(int i = currentSceneObjects.Count - 1; i >=0; i --)
		{
			//GameObject temp = currentSceneObjects[i];
			//currentSceneObjects.RemoveAt(i);
			Destroy(currentSceneObjects[i]);
			currentSceneObjects.RemoveAt(i);
		}
		gameActive = false;
	}


}
