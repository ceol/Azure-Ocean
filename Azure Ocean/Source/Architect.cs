﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureOcean
{
    public class WorldArchitect
    {
        int filledTile = 1;
        int emptyTile = 0;

        Random rand;

        public int[,] GetBaseMap(int width, int height)
        {
            int[,] newMap = new int[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    newMap[x, y] = filledTile;
                }
            }
            return newMap;
        }

        public struct CellularAutomata
        {
            public int percentRandomFilled;

            public int numGenerations;
            public int fillsAt;
            public int emptiesAt;

            public int emptyFillsGenCount;
            public int emptyFillsAt;
        }

        public int[,] GenerateCellularAutomataMap(int width, int height, CellularAutomata cellular)
        {
            int[,] map = GetBaseMap(width, height);

            if (rand == null)
                rand = new Random();

            // Fill initial map randomly
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Borders should be filled
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                        map[x, y] = filledTile;
                    else
                        map[x, y] = rand.Next(0, 100) < cellular.percentRandomFilled ? filledTile : emptyTile;
                }
            }

            // Run cellular automata rules
            // Reversing the loop halfway through attempts to ensure the filled tiles
            // don't look like they're offset or gathering on one side of the map.
            int numGenerations = cellular.numGenerations;
            for (int generation = 0; generation < numGenerations; generation++)
            {
                int coordX = 0;
                int xReverseAt = (int)Math.Floor(width / 2f);
                int coordY = 0;
                int yReverseAt = (int)Math.Floor(height / 2f);
                for (int x = 0; x < width; x++)
                {
                    coordX = x;
                    if (x >= xReverseAt)
                        coordX = width - 1 - (x % xReverseAt);

                    for (int y = 0; y < height; y++)
                    {
                        coordY = y;
                        if (y >= yReverseAt)
                            coordY = height - 1 - (y % yReverseAt);

                        int numFilled = CountSurroundingFilled(map, coordX, coordY, 1);
                        int numFilledWider = CountSurroundingFilled(map, coordX, coordY, 2);

                        // The OR check tries to ensure large empty areas are split up
                        if (numFilled > cellular.fillsAt || (generation < cellular.emptyFillsGenCount && numFilledWider < cellular.emptyFillsAt))
                            map[coordX, coordY] = filledTile;
                        else if (numFilled < cellular.emptiesAt)
                            map[coordX, coordY] = emptyTile;
                    }
                }
            }

            return map;
        }

        public List<Vector> GetConnectedTiles(int[,] map, Vector start)
        {
            int originalTile = map[start.x, start.y];

            Queue<Vector> queue = new Queue<Vector>();
            queue.Enqueue(start);

            Vector current;

            List<Vector> group = new List<Vector>();
            while (queue.Count > 0)
            {
                current = queue.Dequeue();
                group.Add(current);

                foreach (Vector direction in Vector.cardinals)
                {
                    Vector sibling = current + direction;
                    if (IsValid(map, sibling.x, sibling.y) && !group.Contains(sibling) && !queue.Contains(sibling) && map[sibling.x, sibling.y] == originalTile)
                        queue.Enqueue(sibling);
                }
            }

            return group;
        }

        public bool IsValid(int[,] map, int x, int y)
        {
            return (x >= 0 && x < map.GetLength(0) && y >= 0 && y < map.GetLength(1));
        }

        public int[,] GenerateForest(int width, int height)
        {
            // Tile mappings
            int waterTile = 2;

            // This generates a map with a blob-like border to represent
            // the dense trees surrounding the zone.
            CellularAutomata treeCellular = new CellularAutomata()
            {
                percentRandomFilled = 45,
                numGenerations = 7,
                fillsAt = 4,
                emptiesAt = 4,
                emptyFillsGenCount = 0,
                emptyFillsAt = 2,
            };

            // This generates a map of small pools to represent areas of water.
            // These tiles can overwrite empty but not any other.
            CellularAutomata waterCellular = new CellularAutomata()
            {
                percentRandomFilled = 40,
                numGenerations = 4,
                fillsAt = 4,
                emptiesAt = 4,
                emptyFillsGenCount = 0,
                emptyFillsAt = 2,
            };

            // Generate the trees
            int [,] treeMap = GenerateCellularAutomataMap(width, height, treeCellular);

            // Generate pools of water
            filledTile = waterTile;
            int[,] waterMap = GenerateCellularAutomataMap(width, height, waterCellular);
            filledTile = 1;

            int[,] forestMap = PlaceRegion(treeMap, waterMap, 0, 0);

            return forestMap;
        }

        // Paste the region map into the world map starting at the offset coordinate
        public int[,] PlaceRegion(int[,] toMap, int[,] fromMap, int offsetX, int offsetY)
        {
            for (int x = 0; x <= fromMap.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= fromMap.GetUpperBound(1); y++)
                {
                    int realX = x + offsetX;
                    int realY = y + offsetY;

                    if (realX <= toMap.GetUpperBound(0) && realY <= toMap.GetUpperBound(1))
                    {
                        int tile = fromMap[x, y];
                        if (toMap[realX, realY] == emptyTile && tile != emptyTile)
                            toMap[realX, realY] = tile;
                    }
                }
            }

            return toMap;
        }

        public Stage GenerateWorld(int width, int height, string seed)
        {
            rand = new Random(seed.GetHashCode());

            /*
            int numRegions = 0;
            int totalTiles = width * height;
            int tilesPerRegion = (int)Math.Floor((float)totalTiles / numRegions);

            // The goal is to shrink or stretch the dimensions of the region randomly
            // So if we have 1,000 tiles per region, each region might end up being
            // 800 or 1,200 total.
            int baseDimension = (int)Math.Floor(Math.Sqrt(tilesPerRegion));
            float deviationPercent = 30; // how far from the base a region may deviate
            int absDeviation = (int)Math.Floor(baseDimension * (deviationPercent / 100));
            int lowerBound = baseDimension - absDeviation;
            int upperBound = baseDimension + absDeviation;
            
            for (int currentRegion = 0; currentRegion < numRegions; currentRegion++)
            {
                int regionWidth = rand.Next(lowerBound, upperBound);
                int regionHeight = rand.Next(lowerBound, upperBound);

                int[,] regionMap = GenerateDesertRegion(128, 64);

                int randomX = rand.Next(0, width);
                int randomY = rand.Next(0, height);
                map = PlaceRegion(map, regionMap, 0, 0);
            }
            */

            int[,] map = GenerateForest(200, 100);

            Stage stage = new Stage(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == width - 1) { }
                    Vector coordinate = new Vector(x, y);
                    int tile = map[x, y];
                    Tile tileObj;

                    if (tile == emptyTile)
                        tileObj = new GrassTile() { coordinate = coordinate };
                    else if (tile == filledTile)
                        tileObj = new TreeTile() { coordinate = coordinate };
                    else if (tile == 2)
                        tileObj = new WaterTile() { coordinate = coordinate };
                    else
                        tileObj = new StoneTile() { coordinate = coordinate };

                    stage.tiles[x, y] = tileObj;
                }
            }

            return stage;
        }

        public int CountSurroundingFilled(int[,] map, int x, int y, int steps)
        {
            int filledCount = 0;

            for (int neighborX = x - steps; neighborX <= x + steps; neighborX++)
            {
                for (int neighborY = y - steps; neighborY <= y + steps; neighborY++)
                {
                    if (neighborX != x || neighborY != y)
                    {
                        if (IsFilled(map, neighborX, neighborY))
                            filledCount++;
                    }
                }
            }

            return filledCount;
        }

        public bool IsFilled(int[,] map, int x, int y)
        {
            if (IsValid(map, x, y))
                return map[x, y] == filledTile;

            // If we're out of bounds, treat as filled.
            return true;
        }
    }
}
