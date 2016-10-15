using UnityEngine;
using System.Collections;

public class Map {
	private int width;
	private int height;

	private int filled;
	private int empty;

	private int [,] internalMap;

	public Map(int width, int height, int filled, int empty, int[,] map)
	{
		this.width = width;
		this.height = height;
		this.filled = filled;
		this.empty = empty;
		this.internalMap = map;
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

	public int EmptyCode()
	{
		return empty;
	}

	public int Width()
	{
		return width;
	}

	public int Height()
	{
		return height;
	}
}
