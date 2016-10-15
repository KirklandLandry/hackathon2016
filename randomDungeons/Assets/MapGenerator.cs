using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public class MapGenerator {


	private int deathLimit = 3;
	private int birthLimit = 4;
	private int numberOfSimulationSteps = 19;

	private int filled = 0;
	private int empty = 1;

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


		return new Map(size, size, filled, empty, newMap);
	} 


	public void FillRandomly(int[,] map, bool useRandomSeed, int chanceToStartAlive)
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

	public void DoSimulationStep(int[,] map, int size)
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

	public int CountAliveNeighbours(int[,] map, int x, int y, int size)
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
				else if(neighbour_x < 0 || neighbour_x >= size || neighbour_y < 0 || neighbour_y >= size || map[neighbour_y,neighbour_x] == filled)
				{
					count++;
				}
			}
		}
		return count;
	}

}
