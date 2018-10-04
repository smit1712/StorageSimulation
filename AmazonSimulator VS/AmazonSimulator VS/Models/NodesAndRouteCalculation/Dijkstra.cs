using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmazonSimulator 
{
    public class Dijkstra
    {
        private List<Node> NodeList;
        private List<Node> path1;
        private List<Node> path2;
        private List<Node> path3;
        private List<Node> unvisited;
        private List<Node> visited;
        public Dijkstra(List<Node> Nodelist)
        {
            NodeList = Nodelist;
        }
        /// <summary>
        /// Calculates the sortest route from the currentnode to the destination
        /// </summary>
        /// <param name="CurrentNode">Origin or next chosen node</param>
        /// <param name="DestinationNode">Destination</param>
        /// <returns></returns>
        public List<Node> GetBestRoute(Node CurrentNode, Node DestinationNode)
        {
            Node origin = CurrentNode;
            unvisited = NodeList;
            foreach (Node n in unvisited)
            {
                n.SetDistanceFromOrigin(double.PositiveInfinity);
            }
            //Per possible direction shortest path is calculated. and that path is returned
            visited = new List<Node>();
            CurrentNode.SetPreviousNode(CurrentNode.GetAdjacentNode1());
            path1 = ShortestPath(CurrentNode, CurrentNode, DestinationNode, 0, visited);
            visited = new List<Node>();
            CurrentNode.SetPreviousNode(CurrentNode.GetAdjacentNode2());
            path2 = ShortestPath(CurrentNode, CurrentNode, DestinationNode, 0, visited);
            visited = new List<Node>();
            CurrentNode.SetPreviousNode(CurrentNode.GetAdjacentNode3());
            path3 = ShortestPath(CurrentNode, CurrentNode, DestinationNode, 0, visited);
            CurrentNode.SetPreviousNode(null);

            if (path1.Count <= path2.Count && path1.Count < path3.Count)
            {
                return path1;
            }
            if (path2.Count <= path1.Count && path2.Count < path3.Count)
            {
                return path2;
            }
            else
            {
                return path3;
            }
        }
        /// <summary>
        /// Shortest path calculated from currentnode to destination. uses recursion to calculate from node to node
        /// </summary>
        /// <param name="CurrentNode"></param>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <param name="totaldistance"></param>
        /// <param name="visited"></param>
        /// <returns></returns>
        private List<Node> ShortestPath(Node CurrentNode, Node origin, Node destination, double totaldistance, List<Node> visited) {

            if (CurrentNode == destination || visited.Count() > NodeList.Count())
            {
                visited.Add(CurrentNode);
                return visited;
            }

            //Get all adjecent nodes from current node and calculate distance for each adjecent node
            CurrentNode.SetDistanceFromOrigin(CurrentNode.GetDistanceTo(origin));
            visited.Add(CurrentNode);
            Node Adj1 = CurrentNode.GetAdjacentNode1();
            double distance1 = CurrentNode.GetDistanceNode1();
            Node Adj2 = CurrentNode.GetAdjacentNode2();
            double distance2 = CurrentNode.GetDistanceNode2();
            Node Adj3 = new Node(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity, "Adj3");
            double distance3 = double.PositiveInfinity;
           
            if (CurrentNode.GetAdjacentNode3() != null)
            {
                Adj3 = CurrentNode.GetAdjacentNode3();
                distance3 = CurrentNode.GetDistanceNode3();
            }
            if (distance3 + totaldistance <= CurrentNode.GetDistanceFromOrigin())
            {
                CurrentNode.SetDistanceFromOrigin(distance3 + totaldistance);
                Adj3.SetPreviousNode(CurrentNode);
                distance3 = distance3 + totaldistance;
            }
            if (distance1 + totaldistance <= CurrentNode.GetDistanceFromOrigin())
            {
                CurrentNode.SetDistanceFromOrigin(distance1 + totaldistance);
                Adj1.SetPreviousNode(CurrentNode);
                distance1 = distance1 + totaldistance;
            }
            if (distance2 + totaldistance <= CurrentNode.GetDistanceFromOrigin())
            {
                CurrentNode.SetDistanceFromOrigin(distance2 + totaldistance);
                Adj2.SetPreviousNode(CurrentNode);
                distance2 = distance2 + totaldistance;
            }

            //check if z axes is the same as the destination
            if (Adj2.z == destination.z && CurrentNode.GetPreviousNode() != Adj2)
            {
                Adj2.SetPreviousNode(CurrentNode);
                visited = ShortestPath(Adj2, origin, destination, distance2, visited);
                return visited;
            }
            if (Adj3.z == destination.z && CurrentNode.GetPreviousNode() != Adj3)
            {
                Adj3.SetPreviousNode(CurrentNode);
                visited = ShortestPath(Adj3, origin, destination, distance3, visited);
                return visited;
            }
            if (Adj1.z == destination.z && CurrentNode.GetPreviousNode() != Adj1)
            {
                Adj1.SetPreviousNode(CurrentNode);
                visited = ShortestPath(Adj1, origin, destination, distance1, visited);
                return visited;
            }
            //checks if the currentnode is a corner node and choces the best corner node to continue
            if (CurrentNode.type == "adj")
            {
                double adj1ToDestination = double.PositiveInfinity;
                double adj2ToDestination = double.PositiveInfinity;
                double adj3ToDestination = double.PositiveInfinity;
                if (Adj1 != CurrentNode.GetPreviousNode())
                {                    
                    adj1ToDestination = Adj1.GetDistanceTo(destination);
                    if (Adj1.type == "adj")
                    {
                        adj1ToDestination -= 100;
                    }
                }
                if (Adj2 != CurrentNode.GetPreviousNode())
                {
                    adj2ToDestination = Adj2.GetDistanceTo(destination);
                    if (Adj2.type == "adj")
                    {
                        adj2ToDestination -= 100;
                    }
                }
                if (Adj3 != CurrentNode.GetPreviousNode())
                {
                    adj3ToDestination = Adj3.GetDistanceTo(destination);
                    if (Adj3.type == "adj")
                    {
                        adj3ToDestination -= 100;
                    }
                }                                

                if (adj1ToDestination < adj2ToDestination && adj1ToDestination < adj3ToDestination  )
                {
                    Adj1.SetPreviousNode(CurrentNode);
                    visited = ShortestPath(Adj1, origin, destination, distance2, visited);
                    return visited;
                }
                if (adj2ToDestination < adj1ToDestination && adj2ToDestination < adj3ToDestination )
                {
                    Adj2.SetPreviousNode(CurrentNode);
                    visited = ShortestPath(Adj2, origin, destination, distance2, visited);
                    return visited;
                }
                if (adj3ToDestination < adj2ToDestination && adj3ToDestination < adj1ToDestination )
                {
                    Adj3.SetPreviousNode(CurrentNode);
                    visited = ShortestPath(Adj3, origin, destination, distance3, visited);
                    return visited;
                }
            }
            //if nothing else found the best way to continue it calculates shorest distance and choces the shorest way
            List<double> Distances = new List<double>();
            Distances.Add(distance1);
            Distances.Add(distance2);
            Distances.Add(distance3);
            Distances.Sort();
            
            selectNode:
            if (distance2 == Distances[0] && CurrentNode.GetPreviousNode() != Adj2)
            {
                Adj2.SetPreviousNode(CurrentNode);
                visited = ShortestPath(Adj2, origin, destination, distance2, visited);
                return visited;
            }
            if (distance3 == Distances[0] && CurrentNode.GetPreviousNode() != Adj3)
            {
                Adj3.SetPreviousNode(CurrentNode);
                visited = ShortestPath(Adj3, origin, destination, distance3, visited);
                return visited;
            }
            if (distance1 == Distances[0] && CurrentNode.GetPreviousNode() != Adj1)
            {
                Adj1.SetPreviousNode(CurrentNode);
                visited = ShortestPath(Adj1, origin, destination, distance1, visited);
                return visited;
            }

            if (Adj1 == destination)
            {
                return ShortestPath(Adj1, origin, destination, distance1, visited);
            }
            if (Adj2 == destination)
            {
                return ShortestPath(Adj2, origin, destination, distance2, visited);
            }
            if (Adj3 == destination)
            {
                return ShortestPath(Adj3, origin, destination, distance3, visited);
            }

            Distances.RemoveAt(0);
            goto selectNode;
        }
    }
}
