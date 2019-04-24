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
