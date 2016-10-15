using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	private List<GameObject> currentSceneObjects;



	// Use this for initialization
	void Start () 
	{
		currentSceneObjects = new List<GameObject>();
		mapGen = new MapGenerator();
		currentMap = mapGen.NewMap(50, true, 43);
		PlaceEnvironment();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void PlaceEnvironment()
	{
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
		p2Avatar.transform.position = new Vector3(currentMap.StartPos2.x, 0, currentMap.StartPos2.y);


		GameObject p1Beacon = (GameObject)GameObject.Instantiate(spawnBeacon, new Vector3(currentMap.StartPos1.x, 0, currentMap.StartPos1.y), Quaternion.identity, parent.transform);
		currentSceneObjects.Add(p1Beacon);

		GameObject p2Beacon = (GameObject)GameObject.Instantiate(spawnBeacon, new Vector3(currentMap.StartPos2.x, 0, currentMap.StartPos2.y), Quaternion.identity, parent.transform);
		currentSceneObjects.Add(p2Beacon);

	}

	/*
	// for colouring in regions
	void OnDrawGizmos()
	{
		if(currentMap != null)
		{
			for(int y = 0; y < currentMap.Width; y++)
			{
				for(int x = 0; x < currentMap.Height; x++)
				{
					if(currentMap.TileAt(x,y) == currentMap.FilledCode)
						Gizmos.color = Color.black;
					else if(x == currentMap.StartPos1.x && y == currentMap.StartPos1.y)
						Gizmos.color = Color.blue;
					else if(x == currentMap.StartPos2.x && y == currentMap.StartPos2.y)
						Gizmos.color = Color.green;
					else 
						Gizmos.color = Color.white;
					Vector3 pos = new Vector3(-currentMap.Width / 2 + x + 0.5f, 0, -currentMap.Height/2 + y + 0.5f);
					Gizmos.DrawCube(pos, Vector3.one);
				}
			}
		}
	}
	*/
}
