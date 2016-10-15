using UnityEngine;
using System.Collections;


// to advance to the next floor, the player needs to reach the opponents starting position
// just drop a trigger collider on the start positions that says "if opposing player collides, move to next level"

// can venture out to find treasure
// treasure carries on to next floor
// treasure == traps to protect your goals


public class Map {
	private int width;
	private int height;

	private int filled;
	private int empty;
	// treasure
	// enemy spawn pt list

	private int [,] internalMap;

	private Vector2i startPos1;
	private Vector2i startPos2;

	public Map(int width, int height, int filled, int empty, int[,] map)
	{
		this.width = width;
		this.height = height;
		this.filled = filled;
		this.empty = empty;
		this.internalMap = map;

		float interval = (Mathf.Sqrt(width*width + height*height) * 3.0f/4.0f);
		int iterations = 0;
		while(true)
		{
			startPos1 = new Vector2i(UnityEngine.Random.Range(1, width - 1), UnityEngine.Random.Range(1, height - 1));
			startPos2 = new Vector2i(UnityEngine.Random.Range(1, width - 1), UnityEngine.Random.Range(1, height - 1));
			if(map[startPos1.y, startPos1.x] == empty && map[startPos2.y, startPos2.x] == empty)
			{
				if(Mathf.Sqrt( Mathf.Pow(startPos2.x - startPos1.x, 2) + Mathf.Pow(startPos2.y - startPos1.y, 2)) > interval)
				{
					break;
				}
				else 
				{
					interval *= 19/20;
					iterations++;
				}	
			}

			// prevent an infinite loop (shouldn't happen, just being safe)
			if(iterations > 50)
				break;
		}
	}

	public int TileAt(int x, int y)
	{
		return internalMap[y, x];
	}

	public int FilledCode
	{
		get
		{
			return filled;
		}

	}

	public int EmptyCode
	{
		get
		{
			return empty;
		}
	}

	public int Width
	{
		get
		{
			return width;
		}
	}

	public int Height
	{
		get 
		{
			return height;
		}
	}

	public Vector2i StartPos1
	{
		get 
		{
			return startPos1;
		}
	}

	public Vector2i StartPos2
	{
		get 
		{
			return startPos2;
		}
	}

}
