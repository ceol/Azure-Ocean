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
        public StoneTile() { IsTraversable = false; }
    }

    public class TreeTile : Tile
    {
        public TreeTile() { IsTraversable = false; }
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
            return x >= 0 && x <= tiles.GetUpperBound(0) && y >= 0 && y <= tiles.GetUpperBound(1);
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
}
