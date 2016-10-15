using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public class MapGenerator {



	private int deathLimit = 3;
	private int birthLimit = 4;
	private int numberOfSimulationSteps = 9;

	private int filled = -1;
	private int empty = -2;
	// all other numbers must be below this number
	// regions should theoretically be able to increase infinitely
	private int startingRegionsID = 0;

	private string seed = "1";

	public MapGenerator()
	{

	}

	// returns a new map
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
		// remove all regions that're too small
		regionsList = RemoveSmallRegions(newMap, regionsList, 17);
		// sort it so that the larget region is first 
		regionsList.Sort();
		regionsList[0].isMainRegion = true;
		regionsList[0].isAccessibleFromMainRegion = true;

		// make all regions connected
		ConnectClosestRegions(regionsList, newMap, size);


		ConvertMapBackToBasic(newMap, size);


		return new Map(size, size, filled, empty, newMap);
	} 

	// randomly fill a 2D array with either filled or non filled
	private void FillRandomly(int[,] map, bool useRandomSeed, int chanceToStartAlive)
	{
		int width = map.GetLength(1);
		int height = map.GetLength(0);

		if(useRandomSeed)
		{
			seed = System.DateTime.Now.Millisecond.ToString();
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

	// perform another cellular automata simulation step
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

	// check how many alive (filled) regions surround the current tile
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
		

	// use floodfill to detect regions
	private List<Region> DetectRegions(int[,] newMap, int size) 
	{ 
		// count will indicate regions. each region will have it's own number
		int currentRegionID = startingRegionsID;
		//int[,] newMap = oldMap;

		List<Region> regions = new List<Region>();

		// can ignore first and last rows/columns because they're set to always be walls
		// this numbering system is bad / inconstent. change 1 to unfilled, 0 to filled.
		for (int y = 1; y < size - 1; y++)
		{
			for (int x = 1; x < size - 1; x++)
			{
				if(newMap[y,x]==filled)
				{
					// ignore it, it's a wall
				}
				else if (newMap[y, x] == empty) // if it's not already marked as a new region
				{
					// floodfill to create a new region
					Vector2i currentPoint = new Vector2i(x,y);  
					Queue<Vector2i> cellsToCheck = new Queue<Vector2i>();
					cellsToCheck.Enqueue(currentPoint);
					Region newRegion = new Region();

					while(cellsToCheck.Count > 0)
					{ 
						// get the first point
						currentPoint = cellsToCheck.Dequeue();
						// if the dequeued point isn't part of this region

						if (newMap[currentPoint.y, currentPoint.x] != currentRegionID )
						{
							// add the point to the region
							newMap[currentPoint.y, currentPoint.x] = currentRegionID;

							// if it's an edge cell
							if(newMap[currentPoint.y + 1, currentPoint.x] == filled ||
								newMap[currentPoint.y - 1, currentPoint.x] == filled ||
								newMap[currentPoint.y, currentPoint.x + 1] == filled ||
								newMap[currentPoint.y, currentPoint.x - 1] == filled)
							{
								newRegion.AddTile(currentPoint, true);
							}
							else { newRegion.AddTile(currentPoint, false); }


							// because the border is always set to be walls, we don't need to worry about out of bounds errors
							if (newMap[currentPoint.y, currentPoint.x + 1] == empty)
								cellsToCheck.Enqueue(new Vector2i(currentPoint.x + 1, currentPoint.y));

							if (newMap[currentPoint.y, currentPoint.x - 1] == empty)
								cellsToCheck.Enqueue(new Vector2i(currentPoint.x - 1, currentPoint.y));

							if (newMap[currentPoint.y + 1, currentPoint.x] == empty)
								cellsToCheck.Enqueue(new Vector2i(currentPoint.x, currentPoint.y + 1));

							if (newMap[currentPoint.y - 1, currentPoint.x] == empty)
								cellsToCheck.Enqueue(new Vector2i(currentPoint.x, currentPoint.y - 1));
						}
					}
					regions.Add(newRegion);
					currentRegionID++;
				}

			}
		}
		return regions;
	}

	// remove any regions below a certain size threshold
	private List<Region> RemoveSmallRegions(int[,] map, List<Region> regionList, int sizeThreshold)
	{
		List<Region> trimmedRegionList = new List<Region>();
		for(int i = 0; i < regionList.Count; i++)
		{
			if(regionList[i].regionSize <= sizeThreshold)
			{
				for(int j = 0; j < regionList[i].regionSize; j++)
				{
					map[regionList[i].tiles[j].y, regionList[i].tiles[j].x] = filled;
				}
			}
			else 
			{
				trimmedRegionList.Add(regionList[i]);
			}
		}
		return trimmedRegionList;
	}

	// connects regions a and b 
	private void ConnectRegions(Region a, Region b)
	{
		if(a.isAccessibleFromMainRegion)
		{
			b.SetAccessibleFromMainRegion();
		}
		else if(b.isAccessibleFromMainRegion)
		{
			a.SetAccessibleFromMainRegion();
		}
		a.connectedRegions.Add(b);
		b.connectedRegions.Add(a);
	}

	// find and connect regions closest to each other
	// in the first pass when not forcing accessibility to the main region all it will do is connect to the closest region and that's it
	// in the next pass when forcing accessibility to the main region, look for whichever connected room is closest to the main region (but not connected) and connect that one to the main region
	private void ConnectClosestRegions(List<Region> regions, int[,] map, int size, bool forceAccessibleFromMainRegion = false)
	{
		List<Region> roomListA = new List<Region>();
		List<Region> roomListB = new List<Region>();

		if(forceAccessibleFromMainRegion)
		{
			foreach(Region r in regions)
			{
				if(r.isAccessibleFromMainRegion)
				{
					// regions accessible from the main room
					roomListB.Add(r);
				}
				else 
				{
					// regions not accessible from the main room
					roomListA.Add(r);
				}
			}
		}
		else 
		{
			roomListA = regions;
			roomListB = regions;
		}

		int bestDist = 0;
		Vector2i bestTileA = new Vector2i();
		Vector2i bestTileB = new Vector2i();
		Region bestRegionA = new Region();
		Region bestRegionB = new Region();
		bool possibleConnectionFound = false;

		foreach(Region regionA in roomListA)
		{
			// don't want to do this when forcing accessibility because we're not just looking for the first connection
			// we're looking for the closest connection to the main room based on adjacent rooms
			// we're always considering all adjacent rooms before making the connection
			if(!forceAccessibleFromMainRegion)
			{
				possibleConnectionFound = false;
				if(regionA.connectedRegions.Count > 0)
					continue;
			}
			foreach(Region regionB in roomListB)
			{
				// if the regions are equal, don't bother comparing them
				if(regionA == regionB || regionA.IsConnected(regionB))
					continue;

					
				for(int tileIndexA = 0; tileIndexA < regionA.edgeTiles.Count; tileIndexA++)
				{
					for(int tileIndexB = 0; tileIndexB < regionB.edgeTiles.Count; tileIndexB++)
					{
						Vector2i tileA = regionA.edgeTiles[tileIndexA];
						Vector2i tileB = regionB.edgeTiles[tileIndexB];
						int distBetweenRegions = (int)(Mathf.Pow(tileA.x - tileB.x, 2) + Mathf.Pow(tileA.y - tileB.y, 2));

						if(distBetweenRegions < bestDist || !possibleConnectionFound)
						{
							bestDist = distBetweenRegions;
							possibleConnectionFound = true;
							bestTileA = tileA;
							bestTileB = tileB;
							bestRegionA = regionA;
							bestRegionB = regionB;
						}
					}
				}
			}
			if(possibleConnectionFound && !forceAccessibleFromMainRegion)
			{
				CreatePassage(bestRegionA, bestRegionB, bestTileA, bestTileB, map, size);
			}
		}

		// out here because we're considering all adjacent rooms, not just the best room
		if(possibleConnectionFound && forceAccessibleFromMainRegion)
		{
			CreatePassage(bestRegionA, bestRegionB, bestTileA, bestTileB, map, size);
			ConnectClosestRegions(regions, map, size, true);
		}

		if(!forceAccessibleFromMainRegion)
		{
			ConnectClosestRegions(regions, map, size, true);
		}
	}

	private void CreatePassage(Region regionA, Region regionB, Vector2i tileA, Vector2i tileB, int[,] map, int size) 
	{
		ConnectRegions(regionA, regionB);
		//Debug.DrawLine(CoordToWorldPoint(tileA), CoordToWorldPoint(tileB), Color.green, 100);

		List<Vector2i> line = GetLine(tileA, tileB);
		foreach(Vector2i c in line)
		{
			DrawCircle(c, 2, map, size);
		}

	}

	void DrawCircle(Vector2i c, int r, int[,] map, int size)
	{
		for(int x = -r; x <= r; x++)
		{
			for(int y = -r; y <= r; y++)
			{
				if(x*x + y*y <= r*r)
				{
					int drawX = c.x + x;
					int drawY = c.y + y;

					if(IsInMapRange(drawX, drawY, size))
					{
						map[drawY, drawX] = empty;
					}
				}
			}
		}
	}

	// 4:30 am, too tired. will comment later when I can math and also think.
	List<Vector2i> GetLine(Vector2i from, Vector2i to)
	{
		List<Vector2i> line = new List<Vector2i>();

		int x = from.x;
		int y = from.y;

		int dx = to.x - from.x;
		int dy = to.y - from.y;

		bool inverted = false;

		// x step 
		int step = Math.Sign(dx);
		// y step
		int gradientStep = Math.Sign(dy);

		int longest = Mathf.Abs(dx);
		int shortest = Mathf.Abs(dy);

		if(longest < shortest)
		{
			inverted = true;
			longest = Mathf.Abs(dy);
			shortest = Mathf.Abs(dx);

			step = Math.Sign(dy);
			gradientStep = Math.Sign(dx);
		}

		int gradientAccumulation = longest / 2;
		for(int i = 0; i < longest; i++)
		{
			line.Add(new Vector2i(x,y));

			if(inverted) 
			{
				y += step;
			}
			else 
			{
				x += step;
			}

			gradientAccumulation += shortest;
			if(gradientAccumulation >= shortest) 
			{
				if(inverted)
				{
					x+= gradientStep;
				}
				else 
				{
					y += gradientStep;
				}
				gradientAccumulation -= longest;
			}
		}
		return line;
	}

	public void ConvertMapBackToBasic(int[,] map, int size)
	{
		for(int y = 0; y < size; y++)
		{
			for(int x = 0; x < size; x++)
			{
				if(map[y,x] != filled)
				{
					map[y,x] = empty;
				}
			}
		}
	}

	Vector3 CoordToWorldPoint(Vector2i tile)
	{
		return new Vector3(-100 / 2 + 0.5f + tile.x, 2, -100 / 2 + 0.5f + tile.y);
	}

	// is an x,y coord withing the bounds of the map
	private bool IsInMapRange(int x, int y, int size)
	{
		return x >= 0 && x < size && y >= 0 && y < size;
	}

}


public class Region : IComparable<Region>
{
	// not really using this id for anything
	//public int id;

	public List<Vector2i> tiles;
	public List<Vector2i> edgeTiles;
	public List<Region> connectedRegions;
	public int regionSize;
	public bool isAccessibleFromMainRegion;
	public bool isMainRegion;

	public Region()
	{
		this.tiles = new List<Vector2i>();
		this.edgeTiles = new List<Vector2i>();
		this.connectedRegions = new List<Region>();
	}

	/*public Region(int id)
	{
		this.id = id;
		this.tiles = new List<Vector2i>();
		this.edgeTiles = new List<Vector2i>();
		this.connectedRegions = new List<Region>();
	}*/

	public void AddTile(Vector2i tile, bool isEdgeTile)
	{
		tiles.Add(tile);
		if(isEdgeTile)
			edgeTiles.Add(tile);
		regionSize++;
	}

	public bool IsConnected(Region otherRegion)
	{
		return connectedRegions.Contains(otherRegion);
	}

	public void SetAccessibleFromMainRegion()
	{
		if(!isAccessibleFromMainRegion)
		{
			isAccessibleFromMainRegion = true;
			foreach(Region connected in connectedRegions)
			{
				connected.SetAccessibleFromMainRegion();
			}
		}
	}

	public int CompareTo(Region otherRegion)
	{
		return otherRegion.regionSize.CompareTo(regionSize);
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