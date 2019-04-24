using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureOcean
{
    public class HostilePathfinder
    {
        class Node
        {
            public Node parent;

            public Vector vector;

            public int distance;
            public int heuristic;

            public int Score
            {
                get { return distance + heuristic; }
            }

            public int F { get => G + H; }
            public int G { get => distance; set => distance = value; }
            public int H { get => heuristic; set => heuristic = value; }
        }

        Stage stage;

        Vector start;
        Vector destination;

        List<Node> queue = new List<Node>();
        List<Node> visited = new List<Node>();

        int currentDistance;

        public HostilePathfinder(Stage stage)
        {
            this.stage = stage;
        }

        Node CreateNode(Node parent, Vector coordinate)
        {
            return new Node()
            {
                parent = parent,
                vector = coordinate,
                distance = currentDistance,
                heuristic = CalculateHeuristic(coordinate, destination),
            };
        }

        public List<Vector> GetSteps(Vector start, Vector destination)
        {
            this.start = start;
            this.destination = destination;
            currentDistance = 0;

            Node current = CreateNode(null, start);
            queue.Add(current);

            while (queue.Any())
            {
                // Get the next node
                int lowestScore = queue.Min(node => node.F);
                current = queue.First(node => node.F == lowestScore);
                if (current.vector == destination)
                    break;

                queue.Remove(current);

                if (visited.Contains(current))
                    continue;
                visited.Add(current);

                currentDistance++;

                // Create adjacent nodes
                List<Vector> adjacentVectors = stage.GetAdjacentTraversableVectors(current.vector);
                foreach (Vector adjacentVector in adjacentVectors)
                {
                    // ignore if we've already been there
                    if (visited.FirstOrDefault(node => node.vector == adjacentVector) != null)
                        continue;

                    // if it's not in the queue, we should create
                    Node adjacentNode = queue.FirstOrDefault(node => node.vector == adjacentVector);
                    if (adjacentNode == null)
                    {
                        adjacentNode = CreateNode(current, adjacentVector);
                        queue.Add(adjacentNode);
                    }
                    else
                    {
                        // check to see if this is the better path for that node
                        if (currentDistance + adjacentNode.heuristic < adjacentNode.Score)
                        {
                            adjacentNode.distance = currentDistance;
                            adjacentNode.parent = current;
                        }
                    }
                }
            }

            // Walk back from the current node
            List<Vector> steps = new List<Vector>();
            while (current != null && current.vector != start)
            {
                steps.Insert(0, current.vector);
                current = current.parent;
            }

            return steps;
        }

        public int CalculateHeuristic(Vector coordinate, Vector destination)
        {
            return Math.Abs(destination.x - coordinate.x) + Math.Abs(destination.y - coordinate.y);
        }
    }
}
