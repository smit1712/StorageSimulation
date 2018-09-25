using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmazonSimulator 
{
    public class Dijkstra
    {
        private List<Node> NodeList;
        List<Node> path1;
        List<Node> path2;
        List<Node> unvisited;
        List<Node> visited;
        public Dijkstra(List<Node> Nodelist)
        {
            NodeList = Nodelist;
        }

        public List<Node> GetBestRoute(Node CurrentNode, Node DestinationNode)
        {
            Node origin = CurrentNode;
            unvisited = NodeList;
            foreach (Node n in unvisited)
            {
                n.SetDistanceFromOrigin(double.PositiveInfinity);
            }
            
            visited = new List<Node>();

            path1 = ShortestPath(CurrentNode, CurrentNode, DestinationNode, 0, visited, unvisited);
            visited = new List<Node>();
            path2 = ShortestPath(CurrentNode, CurrentNode, DestinationNode, 0, visited, unvisited);
            while(path1.Count != path2.Count)
            {
               
                    path1 = ShortestPath(CurrentNode, CurrentNode, DestinationNode, 0, visited, unvisited);
                    visited = new List<Node>();
                    path2 = ShortestPath(CurrentNode, CurrentNode, DestinationNode, 0, visited, unvisited);
                    visited = new List<Node>();

                

            }
            return path1;

        }

        private List<Node> ShortestPath(Node CurrentNode, Node origin, Node destination, double totaldistance, List<Node> visited, List<Node> unvisited) {

            if (CurrentNode == destination)
            {
                visited.Add(CurrentNode);
                return visited;
            }


            CurrentNode.SetDistanceFromOrigin(CurrentNode.GetDistanceTo(origin));
            visited.Add(CurrentNode);
            Node Adj1 = CurrentNode.GetAdjacentNode1();
            double distance1 = CurrentNode.GetDistanceNode1();
            Node Adj2 = CurrentNode.GetAdjacentNode2();
            double distance2 = CurrentNode.GetDistanceNode2();
            Node Adj3 = new Node(0, 0, 0, "Adj3");
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
    
            List<double> Distances = new List<double>();
            Distances.Add(distance1);
            Distances.Add(distance2);
            Distances.Add(distance3);
            Distances.Sort();
            
            selectNode:

            if (distance1 == Distances[0] && CurrentNode.GetPreviousNode() != Adj1)
            {
                Adj1.SetPreviousNode(CurrentNode);
                visited = ShortestPath(Adj1, origin, destination, distance1, visited, unvisited);
                return visited;
            }
            if (distance2 == Distances[0] && CurrentNode.GetPreviousNode() != Adj2)
            {
                Adj2.SetPreviousNode(CurrentNode);
                visited = ShortestPath(Adj2, origin, destination, distance2, visited, unvisited);
                return visited;
            }
            if (distance3 == Distances[0] && CurrentNode.GetPreviousNode() != Adj3)
            {
                Adj3.SetPreviousNode(CurrentNode);
                visited = ShortestPath(Adj3, origin, destination, distance3, visited, unvisited);
                return visited;
            }



            if (Adj1 == destination)
            {
                return ShortestPath(Adj1, origin, destination, distance1, visited, unvisited);
            }
            if (Adj2 == destination)
            {
                return ShortestPath(Adj2, origin, destination, distance2, visited, unvisited);
            }
            if (Adj3 == destination)
            {
                return ShortestPath(Adj3, origin, destination, distance3, visited, unvisited);
            }

            Distances.RemoveAt(0);
            goto selectNode;
            return visited;
         

            //unvisited.Remove(CurrentNode);
            //foreach(Node n in unvisited)
            //{
            //    n.SetDistanceFromOrigin(double.PositiveInfinity);
            //}
            //List<Node> BestRoute = new List<Node>();          

            ////List<Node> unvisited = NodeList;

            //Node adjacent1 = CurrentNode.GetAdjacentNode1();
            //Node adjacent2 = CurrentNode.GetAdjacentNode2();
            //Node adjacent3 = CurrentNode.GetAdjacentNode3();


            //Double Distance1, Distance2, Distance3 = double.PositiveInfinity;

            //Distance1 = CurrentNode.GetDistanceTo(adjacent1) + DistanceFromOrigin;
            //Distance2 = CurrentNode.GetDistanceTo(adjacent2) + DistanceFromOrigin;
            //if (adjacent3 != null)
            //{
            //    Distance3 = CurrentNode.GetDistanceTo(adjacent3) + DistanceFromOrigin;
            //}

            //if (Distance1 < adjacent1.GetDistanceFromOrigin())
            //{
            //    adjacent1.SetDistanceFromOrigin(Distance1);
            //}
            //if (Distance2 < adjacent2.GetDistanceFromOrigin())
            //{
            //    adjacent2.SetDistanceFromOrigin(Distance2);
            //}
            //if (adjacent3 != null && Distance3 < adjacent3.GetDistanceFromOrigin())
            //{
            //    adjacent3.SetDistanceFromOrigin(Distance3);
            //}



            //List<double> Distances = new List<double>();
            //Distances.Add(Distance1);
            //Distances.Add(Distance2);
            //Distances.Add(Distance3);
            //Distances.Sort();
            //Distances.Reverse();
            //unvisited.Remove(CurrentNode);
            //visited.Add(CurrentNode);

            //if(Distance1 == Distances[0] && adjacent1.GetDistanceFromOrigin() <= Distance1 && !visited.Contains(adjacent1))
            //{
            //    //adjacent1.SetDistanceFromOrigin(Distance1);
            //    BestRoute.AddRange(GetBestRoute(adjacent1, DestinationNode, origin, Distance1, visited, unvisited));                
            //    return BestRoute;
            //}
            //if (Distance2 == Distances[0] && adjacent2.GetDistanceFromOrigin() <= Distance2 && !visited.Contains(adjacent2))
            //{
            //    //adjacent2.SetDistanceFromOrigin(Distance2);
            //    BestRoute.AddRange(GetBestRoute(adjacent2, DestinationNode, origin, Distance2, visited, unvisited));
            //    return BestRoute;
            //}
            //if (adjacent3 != null)
            //{
            //    if (Distance3 == Distances[0] && adjacent3.GetDistanceFromOrigin() <= Distance3 && !visited.Contains(adjacent3))
            //    {
            //        //adjacent3.SetDistanceFromOrigin(Distance3);
            //        BestRoute.AddRange(GetBestRoute(adjacent3, DestinationNode, origin, Distance3, visited, unvisited));
            //        return BestRoute;
            //    }
            //}

            //if (CurrentNode == DestinationNode)
            //{

            //    visited.Add(CurrentNode);
            //    return visited;
            //}
            //else
            //{
            //    visited = new List<Node>();
            //   //unvisited = NodeList;
            //    List<Node> newroute = GetBestRoute(origin, DestinationNode, origin, 0, visited, unvisited);
            //    return newroute;

            //}

        }
    }
}
