using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Debug = System.Diagnostics.Debug;

namespace AzureOcean
{
    public class AStarPathfinder
    {
        class Node : IComparable<Node>
        {
            public Node parent;

            public Vector vector;

            public int distance;
            public int heuristic;

            public int priority
            {
                get { return distance + heuristic; }
            }

            public int F { get => distance + heuristic; }
            public int G { get => distance; set => distance = value; }
            public int H { get => heuristic; set => heuristic = value; }

            public int CompareTo(Node other)
            {
                if (this.priority < other.priority) return -1;
                else if (this.priority > other.priority) return 1;
                else return 0;
            }
        }

        Stage stage;

        Vector start;
        Vector destination;

        PriorityQueue<Node> queue = new PriorityQueue<Node>();
        List<Vector> visited = new List<Vector>();

        int currentDistance;

        public AStarPathfinder(Stage stage)
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
            queue.Enqueue(current);

            while (queue.Count > 0)
            {
                // Get the next node
                current = queue.Dequeue();
                if (current.vector == destination)
                    break;

                if (visited.Contains(current.vector))
                    continue;
                visited.Add(current.vector);

                currentDistance++;

                // Create adjacent nodes
                List<Vector> adjacentVectors = stage.GetAdjacentTraversableVectors(current.vector);
                foreach (Vector adjacentVector in adjacentVectors)
                {
                    // ignore if we've already been there
                    if (visited.Contains(adjacentVector))
                        continue;

                    Node adjacentNode = SearchQueue(adjacentVector);
                    if (adjacentNode == null)
                    {
                        adjacentNode = CreateNode(current, adjacentVector);
                        queue.Enqueue(adjacentNode);
                    }
                    else
                    {
                        if (currentDistance + adjacentNode.heuristic < adjacentNode.priority)
                        {
                            adjacentNode.distance = currentDistance;
                            Requeue(adjacentNode);
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

        Node SearchQueue(Vector vector)
        {
            foreach (Node node in queue.GetQueue())
            {
                if (node.vector == vector)
                    return node;
            }

            return null;
        }

        void Requeue(Node updatedNode)
        {
            queue.Remove(updatedNode);
            queue.Enqueue(updatedNode);
        }

        // Heuristic can be anything that signifies the tile's "cost" to get
        // to the destination relative to the others. For this, we just use
        // the magnitude squared.
        public int CalculateHeuristic(Vector coordinate, Vector destination)
        {
            return (int)(Math.Pow(Math.Abs(destination.x - coordinate.x), 2) + Math.Pow(Math.Abs(destination.y - coordinate.y), 2));
        }
    }
}
