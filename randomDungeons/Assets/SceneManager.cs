using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour {

	MapGenerator mapGen;
	Map currentMap;

	// Use this for initialization
	void Start () 
	{
		mapGen = new MapGenerator();
		currentMap = mapGen.NewMap(100, true, 43);
	}
	
	// Update is called once per frame
	void Update () 
	{
		print(currentMap.Width());
		if(Input.GetKeyDown(KeyCode.F))
		{
			currentMap = mapGen.NewMap(100, true, 43);
		}
	}

	void OnDrawGizmos()
	{
		if(currentMap != null)
		{
			for(int y = 0; y < currentMap.Width(); y++)
			{
				for(int x = 0; x < currentMap.Height(); x++)
				{
					Gizmos.color = currentMap.TileAt(x,y) == currentMap.FilledCode ? Color.black : Color.white;
					Vector3 pos = new Vector3(-currentMap.Width() / 2 + x + 0.5f, 0, -currentMap.Height()/2 + y + 0.5f);
					Gizmos.DrawCube(pos, Vector3.one);
				}
			}
		}
	}

}
