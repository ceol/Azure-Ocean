﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Random = System.Random;

namespace AzureOcean
{
    public class Tile
    {

    }

    public class GrassTile : Tile
    {

    }

    public class WaterTile : Tile
    {

    }

    public class Stage
    {
        public int width;
        public int height;

        public Tile[,] tiles;

        public Stage(int width, int height)
        {
            this.width = width;
            this.height = height;
            tiles = new Tile[width, height];
        }
    }

    public class Architect
    {
        int[,] map;

        // Instead of "wall" and "floor" (or "not wall") used in most cellular
        // automata generations, we're just renaming them to "water" and
        // "grass", respectively, to be more obvious.
        int waterTile = 1;
        int grassTile = 0;

        public Stage GenerateWorld(int width, int height, string seed)
        {
            // Used for generation processes then converted to Stage at the end.
            map = new int[width, height];

            // Randomly fill stage
            Random rand = new Random(seed.GetHashCode());
            int percentWaterChance = 45;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Borders should be water
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                        map[x, y] = waterTile;
                    else
                        map[x, y] = rand.Next(0, 100) < percentWaterChance ? waterTile : grassTile;
                }
            }

            // Smooth randomly generated stage with cellular automata rules
            int numGenerations = 7;
            for (int generation = 0; generation < numGenerations; generation++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int numWaters = CountSurroundingWaters(x, y, 1);
                        int numWaters2Step = CountSurroundingWaters(x, y, 2);

                        // The OR check tries to ensure large open areas are split up by water
                        if (numWaters > 4 || (generation < 4 && numWaters2Step < 2))
                            map[x, y] = waterTile;
                        else if (numWaters <= 4)
                            map[x, y] = grassTile;
                    }
                }
            }

            // Convert tile map to stage
            Stage stage = new Stage(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map[x, y] == waterTile)
                        stage.tiles[x, y] = new WaterTile();
                    else if (map[x, y] == grassTile)
                        stage.tiles[x, y] = new GrassTile();
                }
            }

            return stage;
        }

        public int CountSurroundingWaters(int x, int y, int steps)
        {
            int waterCount = 0;

            for (int neighborX = x - steps; neighborX <= x + steps; neighborX++)
            {
                for (int neighborY = y - steps; neighborY <= y + steps; neighborY++)
                {
                    if (IsWater(neighborX, neighborY))
                        waterCount++;
                }
            }

            return waterCount;
        }

        public bool IsWater(int x, int y)
        {
            if (x >= 0 && x < map.GetUpperBound(0) && y >= 0 && y < map.GetUpperBound(1))
                return map[x, y] == waterTile;

            // If we're out of bounds, treat as water.
            return true;
        }
    }
}
