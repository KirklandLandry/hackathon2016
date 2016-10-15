﻿using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public class MapGenerator {



	private int deathLimit = 3;
	private int birthLimit = 4;
	private int numberOfSimulationSteps = 19;

	private int filled = -1;
	private int empty = -2;
	// all other numbers must be below this number
	// regions should theoretically be able to increase infinitely
	private int startingRegionsID = 0;

	private string seed = "1";

	public MapGenerator()
	{

	}

	public Map NewMap(int size, bool useRandomSeed, int chanceToStartAlive)
	{
		
		int[,] newMap = new int[size, size];

		
		FillRandomly(newMap, useRandomSeed, chanceToStartAlive);

		for(int i = 0; i < numberOfSimulationSteps; i ++)
		{
			DoSimulationStep(newMap, size);
		}

		// detect the regions (concave sets of empty tiles)
		List<Region> regionsList = DetectRegions(newMap, size);

		return new Map(size, size, filled, empty, newMap);
	} 


	private void FillRandomly(int[,] map, bool useRandomSeed, int chanceToStartAlive)
	{
		int width = map.GetLength(1);
		int height = map.GetLength(0);

		if(useRandomSeed)
		{
			seed = System.DateTime.Now.ToString();
		}

		System.Random prng = new System.Random(seed.GetHashCode());

		for(int y = 0; y < height; y++)
		{
			for(int x = 0; x < width; x++)
			{
				if(x == 0 || x == (width - 1) || y == 0 || y == (height - 1))
				{
					map[y, x] = filled;
				}
				else 
				{
					map[y, x] = prng.Next(0,100) < chanceToStartAlive ? filled : empty;
				}
			}
		}
	}

	private void DoSimulationStep(int[,] map, int size)
	{
		for(int y = 0; y < size; y++)
		{
			for(int x = 0; x < size; x++)
			{
				// get the # of alive neighbours for the current cell 
				int aliveNeighbourCount = CountAliveNeighbours(map, x, y, size);

				// always guarentee edge wall
				if(y == 0 || x == 0 || x == (size - 1) || y == (size - 1))
				{
					map[y, x] = filled;
				}
				// if this cell is alive ...
				else if(map[y,x] == filled)
				{
					// and it doesn't have enough living neighbours
					if(aliveNeighbourCount < deathLimit)
					{
						// it dies 
						map[y, x] = empty;
					}
					else 
					{
						// it lives
						map[y, x] = filled;
					}
				}
				// if this cell is dead ...
				else 
				{
					// and it has enough living neighbours ...
					if(aliveNeighbourCount > birthLimit)
					{
						// it lives 
						map[y, x] = filled;
					}
					else 
					{
						map[y, x] = empty;
					}
				}
			}
		}
	}

	private int CountAliveNeighbours(int[,] map, int x, int y, int size)
	{
		// getting the total count of alive neighbours
		int count = 0;        
		// n = neighbour, c = current cell
		// n n n
		// n c n
		// n n n
		for (int i = -1; i < 2; i++)
		{
			for (int j = -1; j < 2; j++)
			{
				int neighbour_x = x + j;
				int neighbour_y = y + i;
				//ignore the middle point (the current point)
				if (i == 0 && j == 0) 
				{ 
				}
				// if we're looking at or past a map edge OR if it's just a normal cell
				else if(!IsInMapRange(neighbour_x, neighbour_y, size) || map[neighbour_y,neighbour_x] == filled)
				{
					count++;
				}
			}
		}
		return count;
	}

	/*List<Vector2i> GetRegionTiles(int startX, int startY, int[,] map)
	{

	}*/


	private List<Region> DetectRegions(int[,] map, int size )
	{
		int currentRegionID = startingRegionsID;
		List<Region> regions = new List<Region>();

		for(int y = 1; y < size - 1; y++)
		{
			for(int x = 1; x < size - 1; x++)
			{
				if(map[y, x] == 1)
				{
					// ignore walls
				}
				// if the tile is not a wall and not marked as a region yet
				else if(map[y, x] == empty)
				{
					// floodfill for new region creation

					Vector2i currentCell = new Vector2i(x, y);
					Queue<Vector2i> cellsToCheck = new Queue<Vector2i>();
					cellsToCheck.Enqueue(currentCell);
					Region newRegion = new Region(currentRegionID);


					while(cellsToCheck.Count > 0)
					{
						// get the current cell
						currentCell = cellsToCheck.Dequeue();

						// if the dequeued cell isn't already a part of this region
						if(map[currentCell.y, currentCell.x] != currentRegionID)
						{
							// add current point to the region
							map[currentCell.y, currentCell.x] = currentRegionID;

							// if it's an edge cell
							if(map[currentCell.y + 1, currentCell.x] == filled ||
								map[currentCell.y - 1, currentCell.x] == filled ||
								map[currentCell.y, currentCell.x + 1] == filled ||
								map[currentCell.y, currentCell.x - 1] == filled)
							{
								newRegion.AddTile(currentCell, true);
							}
							// else it's not an edge tile
							else 
							{
								newRegion.AddTile(currentCell, false);
							}

							// because the border is always set to be walls, we don't need to worry about out of bounds errors
							if(map[currentCell.y + 1, currentCell.x] == empty)
								cellsToCheck.Enqueue(new Vector2i(currentCell.y + 1, currentCell.x));

							if(map[currentCell.y - 1, currentCell.x] == empty)
								cellsToCheck.Enqueue(new Vector2i(currentCell.y - 1, currentCell.x));

							if(map[currentCell.y, currentCell.x + 1] == empty)
								cellsToCheck.Enqueue(new Vector2i(currentCell.y, currentCell.x + 1));

							if(map[currentCell.y, currentCell.x - 1] == empty)
								cellsToCheck.Enqueue(new Vector2i(currentCell.y, currentCell.x - 1));

						}
					}
					regions.Add(newRegion);
					currentRegionID++;
				}
			}
		}
		return regions;
	}

	private bool IsInMapRange(int x, int y, int size)
	{
		return x >= 0 && x < size && y >= 0 && y < size;
	}

}


public class Region
{
	public int id;

	public List<Vector2i> tiles;
	public List<Vector2i> edgeTiles;
	public List<Region> connectedRegions;
	public int roomSize;

	public Region(int id)
	{
		this.id = id;
		this.tiles = new List<Vector2i>();
		this.edgeTiles = new List<Vector2i>();
		this.connectedRegions = new List<Region>();
	}

	public void AddTile(Vector2i tile, bool isEdgeTile)
	{
		tiles.Add(tile);
		if(isEdgeTile)
			edgeTiles.Add(tile);
	}
}

public struct Vector2i
{
	public int x,y;
	public Vector2i(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
}