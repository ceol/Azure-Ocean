using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Random = System.Random;

namespace AzureOcean
{
    public class Tile
    {
        public bool IsTraversable = true;
        public Vector coordinate;
    }

    public class GrassTile : Tile
    {

    }

    public class WaterTile : Tile
    {
        public WaterTile() { IsTraversable = false; }
    }

    public class DesertTile : Tile
    {

    }

    public class StoneTile : Tile
    {
        
    }

    public class Stage
    {
        public int width;
        public int height;

        public Tile[,] tiles;

        Dictionary<Entity, Vector> entityPositions = new Dictionary<Entity, Vector>();

        public Stage(int width, int height)
        {
            this.width = width;
            this.height = height;
            tiles = new Tile[width, height];
        }

        public Tile GetTile(Vector coordinate)
        {
            if (IsValid(coordinate))
                return tiles[coordinate.x, coordinate.y];

            return null;
        }

        public bool IsTraversable(Vector coordinate)
        {
            return IsValid(coordinate) && tiles[coordinate.x, coordinate.y].IsTraversable;
        }

        public bool IsValid(Vector coordinate)
        {
            return IsValid(coordinate.x, coordinate.y);
        }

        public bool IsValid(int x, int y)
        {
            return x >= 0 && x < tiles.GetUpperBound(0) && y >= 0 && y < tiles.GetUpperBound(1);
        }

        public void MoveTo(Entity entity, Vector position)
        {
            entityPositions[entity] = position;
        }

        public Entity GetOccupant(Vector coordinate)
        {
            foreach (KeyValuePair<Entity, Vector> pair in entityPositions)
            {
                if (pair.Value == coordinate)
                    return pair.Key;
            }
            return null;
        }

        public void SetOccupant(Entity entity)
        {
            Components.Transform transform = entity.GetComponent<Components.Transform>();
            if (transform != null)
            {
                MoveTo(entity, transform.position);
            }
        }

        public void RemoveEntity(Entity entity)
        {
            entityPositions.Remove(entity);
        }

        public List<Vector> GetAdjacentTraversableVectors(Vector coordinate)
        {
            List<Vector> neighbors = new List<Vector>();

            int steps = 1;

            for (int x = coordinate.x - steps; x <= coordinate.x + steps; x++)
            {
                for (int y = coordinate.y - steps; y <= coordinate.y + steps; y++)
                {
                    // only do cardinal directions
                    if ((x == coordinate.x || y == coordinate.y) && IsValid(x, y) && tiles[x, y].IsTraversable)
                    {
                        neighbors.Add(new Vector(x, y));
                    }
                }
            }

            return neighbors;
        }
    }

    public class WorldArchitect
    {
        int waterTile = 1; // "wall" tile
        int grassTile = 0; // "floor" tile

        Random rand;

        public int[,] GetMap(int width, int height)
        {
            int[,] newMap = new int[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    newMap[x, y] = waterTile;
                }
            }
            return newMap;
        }

        public struct CellularAutomata
        {
            public int percentRandomWater;

            public int numGenerations;
            public int becomesWaterAt;
            public int becomesGrassAt;

            public int secondaryFillUntil;
            public int secondaryFillLowerBound;
        }

        public int[,] GenerateRegion(int width, int height, CellularAutomata cellular)
        {
            int[,] regionMap = GetMap(width, height);
            int totalTiles = width * height;

            if (rand == null)
                rand = new Random();

            // Fill initial map randomly
            int percentWaterChance = cellular.percentRandomWater;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Borders should be water
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                        regionMap[x, y] = waterTile;
                    else
                        regionMap[x, y] = rand.Next(0, 100) < percentWaterChance ? waterTile : grassTile;
                }
            }

            // Run cellular automata rules
            // Reversing the loop halfway through attempts to ensure the tiles
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
                        coordX = width - 1 - (x - xReverseAt);

                    for (int y = 0; y < height; y++)
                    {
                        coordY = y;
                        if (y >= yReverseAt)
                            coordY = height - 1 - (y - yReverseAt);

                        int numWaters = CountSurroundingWaters(regionMap, coordX, coordY, 1);
                        int numWaters2Step = CountSurroundingWaters(regionMap, coordX, coordY, 2);

                        // The OR check tries to ensure large open areas are split up by water
                        if (numWaters > cellular.becomesWaterAt || (generation < cellular.secondaryFillUntil && numWaters2Step < cellular.secondaryFillLowerBound))
                            regionMap[coordX, coordY] = waterTile;
                        else if (numWaters < cellular.becomesGrassAt)
                            regionMap[coordX, coordY] = grassTile;
                    }
                }
            }

            return regionMap;
        }

        public int[,] GenerateIslandRegion(int width, int height)
        {
            CellularAutomata cellular = new CellularAutomata()
            {
                percentRandomWater = 48,
                numGenerations = 7,
                becomesWaterAt = 4,
                becomesGrassAt = 3,
                secondaryFillUntil = 4,
                secondaryFillLowerBound = 3,
            };
            int[,] map = GetMap(width, height);

            for (int i = 0; i < 3; i++)
            {
                PlaceRegion(map, GenerateRegion(width, height, cellular), 0, 0);
            }

            return map;
        }

        public int[,] GenerateForestRegion(int width, int height)
        {
            CellularAutomata cellular = new CellularAutomata()
            {
                percentRandomWater = 47,
                numGenerations = 10,
                becomesWaterAt = 4,
                becomesGrassAt = 3,
                secondaryFillUntil = 4,
                secondaryFillLowerBound = 3,
            };
            int[,] map = GetMap(width, height);

            for (int i = 0; i < 1; i++)
            {
                PlaceRegion(map, GenerateRegion(width, height, cellular), 0, 0);
            }

            for (int x = 0; x < map.GetUpperBound(0); x++)
            {
                for (int y = 0; y < map.GetUpperBound(1); y++)
                {
                    if (map[x, y] == grassTile)
                        map[x, y] = waterTile;
                    else if (map[x, y] == waterTile)
                        map[x, y] = grassTile;
                }
            }

            return map;
        }

        public int[,] GenerateDesertRegion(int width, int height)
        {
            CellularAutomata cellular = new CellularAutomata()
            {
                percentRandomWater = 50,
                numGenerations = 10,
                becomesWaterAt = 4,
                becomesGrassAt = 3,
                secondaryFillUntil = 4,
                secondaryFillLowerBound = 2,
            };
            int[,] map = GetMap(width, height);

            for (int i = 0; i < 1; i++)
            {
                PlaceRegion(map, GenerateRegion(width, height, cellular), 0, 0);
            }

            for (int x = 0; x < map.GetUpperBound(0); x++)
            {
                for (int y = 0; y < map.GetUpperBound(1); y++)
                {
                    if (map[x, y] == grassTile)
                        map[x, y] = waterTile;
                    else if (map[x, y] == waterTile)
                        map[x, y] = grassTile;
                }
            }

            return map;
        }

        public int[,] GenerateSnowRegion(int width, int height)
        {
            CellularAutomata cellular = new CellularAutomata()
            {
                percentRandomWater = 50,
                numGenerations = 10,
                becomesWaterAt = 4,
                becomesGrassAt = 3,
                secondaryFillUntil = 4,
                secondaryFillLowerBound = 2,
            };
            int[,] map = GetMap(width, height);

            for (int i = 0; i < 1; i++)
            {
                PlaceRegion(map, GenerateRegion(width, height, cellular), 0, 0);
            }

            for (int x = 0; x < map.GetUpperBound(0); x++)
            {
                for (int y = 0; y < map.GetUpperBound(1); y++)
                {
                    if (map[x, y] == grassTile)
                        map[x, y] = waterTile;
                    else if (map[x, y] == waterTile)
                        map[x, y] = grassTile;
                }
            }

            return map;
        }

        public int[,] GenerateMountainRegion(int width, int height)
        {
            CellularAutomata cellular = new CellularAutomata()
            {
                percentRandomWater = 45,
                numGenerations = 7,
                becomesWaterAt = 4,
                becomesGrassAt = 4,
                secondaryFillUntil = 4,
                secondaryFillLowerBound = 2,
            };
            int[,] map = GetMap(width, height);

            for (int i = 0; i < 1; i++)
            {
                PlaceRegion(map, GenerateRegion(width, height, cellular), 0, 0);
            }

            return map;
        }

        // Paste the region map into the world map starting at the offset coordinate
        public void PlaceRegion(int[,] map, int[,] regionMap, int offsetX, int offsetY)
        {
            for (int x = 0; x < regionMap.GetUpperBound(0); x++)
            {
                for (int y = 0; y < regionMap.GetUpperBound(1); y++)
                {
                    int realX = x + offsetX;
                    int realY = y + offsetY;

                    if (realX < map.GetUpperBound(0) && realY < map.GetUpperBound(1))
                    {
                        int tile = regionMap[x, y];
                        if (tile == grassTile)
                            map[realX, realY] = tile;
                    }
                }
            }
        }

        public Stage GenerateWorld(int width, int height, string seed)
        {
            int[,] map = GetMap(width, height);
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
                PlaceRegion(map, regionMap, 0, 0);
            }
            */

            PlaceRegion(map, GenerateMountainRegion(200, 100), 0, 0);

            Stage stage = new Stage(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector coordinate = new Vector(x, y);
                    if (map[x, y] == waterTile)
                        stage.tiles[x, y] = new WaterTile() { coordinate = coordinate };
                    else if (map[x, y] == grassTile)
                        stage.tiles[x, y] = new GrassTile() { coordinate = coordinate };
                }
            }

            return stage;
        }

        public int CountSurroundingWaters(int[,] map, int x, int y, int steps)
        {
            int waterCount = 0;

            for (int neighborX = x - steps; neighborX <= x + steps; neighborX++)
            {
                for (int neighborY = y - steps; neighborY <= y + steps; neighborY++)
                {
                    if (neighborX != x || neighborY != y)
                    {
                        if (IsWater(map, neighborX, neighborY))
                            waterCount++;
                    }
                }
            }

            return waterCount;
        }

        public bool IsWater(int[,] map, int x, int y)
        {
            if (x >= 0 && x < map.GetUpperBound(0) && y >= 0 && y < map.GetUpperBound(1))
                return map[x, y] == waterTile;

            // If we're out of bounds, treat as water.
            return true;
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
            int percentWaterChance = 48;
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
                    Vector coordinate = new Vector(x, y);
                    if (map[x, y] == waterTile)
                        stage.tiles[x, y] = new WaterTile() { coordinate = coordinate };
                    else if (map[x, y] == grassTile)
                        stage.tiles[x, y] = new GrassTile() { coordinate = coordinate };
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
