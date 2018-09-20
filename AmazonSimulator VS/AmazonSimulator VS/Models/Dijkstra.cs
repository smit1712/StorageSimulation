using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmazonSimulator 
{
    public class Dijkstra
    {
        List<Node> NodeList;
        public Dijkstra(List<Node> Nodelist)
        {
            NodeList = Nodelist;
        }

        public List<Node> GetBestRoute(Node CurrentNode, Node DestinationNode, Node Lastnode)
        {
            Node adjectent1 = CurrentNode.GetAdjacentNode1();
            Node adjectent2 = CurrentNode.GetAdjacentNode2();
            List<Node> BestRoute = new List<Node>();
            double Totaldistance1 = GetTotalDistance(adjectent1, DestinationNode);
            double Totaldistance2 = GetTotalDistance(adjectent2, DestinationNode);
            if (CurrentNode == DestinationNode)
            {
                BestRoute.Add(CurrentNode);
                return BestRoute;
            }
            else
            if (Totaldistance1 < Totaldistance2 && adjectent1 != Lastnode)
            {
                BestRoute.AddRange(GetBestRoute(adjectent1, DestinationNode,CurrentNode));
            }
            else
            {
                BestRoute.AddRange(GetBestRoute(adjectent2, DestinationNode,CurrentNode));
            }
            BestRoute.Add(CurrentNode);
           // BestRoute.Reverse();
            return BestRoute;
        }

        public double GetTotalDistance(Node CurrentNode, Node DestinationNode)
        {
            List<Node> Unvisited = NodeList;
            Unvisited.Remove(CurrentNode);
            List<Node> Visited = new List<Node>();
            
            double Routelength = 0;

            while (CurrentNode.NodeName != DestinationNode.NodeName)
            {
                Visited.Add(CurrentNode);

                if (CurrentNode.GetAdjacentNode1() == DestinationNode)
                {
                    Routelength = Routelength + CurrentNode.GetDistanceNode1();
                    CurrentNode = CurrentNode.GetAdjacentNode1();
                }
                else
                if (CurrentNode.GetAdjacentNode2() == DestinationNode)
                {
                    Routelength = Routelength + CurrentNode.GetDistanceNode2();
                    CurrentNode = CurrentNode.GetAdjacentNode2();
                }
                else
                if (CurrentNode.GetDistanceNode1() < CurrentNode.GetDistanceNode2() && !Visited.Contains(CurrentNode.GetAdjacentNode1()))
                {
                    Routelength = Routelength + CurrentNode.GetDistanceNode1();
                    CurrentNode = CurrentNode.GetAdjacentNode1();
                }
                else
                {
                    Routelength = Routelength + CurrentNode.GetDistanceNode2();
                    CurrentNode = CurrentNode.GetAdjacentNode2();
                }
            }

            if (CurrentNode.NodeName == DestinationNode.NodeName)
            {
                Visited.Add(CurrentNode);
            }

            return Routelength;

        }
        //        public List<Node> GetRoute(Node CurrentNode, Node DestinationNode)
        //    {
        //        List<Node> Unvisited = NodeList;
        //        Unvisited.Remove(CurrentNode);           
        //        List<Node> Visited = new List<Node>();           
        //        List<Node> Route = new List<Node>();

        //        double Routelength = 0;

        //        while (CurrentNode.NodeName != DestinationNode.NodeName)
        //        {
        //            Visited.Add(CurrentNode);

        //            if(CurrentNode.GetAdjacentNode1() == DestinationNode)
        //            {
        //                Routelength = Routelength + CurrentNode.GetDistanceNode1();
        //                CurrentNode = CurrentNode.GetAdjacentNode1();
        //            }
        //            else
        //            if (CurrentNode.GetAdjacentNode2() == DestinationNode)
        //            {
        //                Routelength = Routelength + CurrentNode.GetDistanceNode2();
        //                CurrentNode = CurrentNode.GetAdjacentNode2();
        //            }
        //            else
        //            if (CurrentNode.GetDistanceNode1() < CurrentNode.GetDistanceNode2() && !Visited.Contains(CurrentNode.GetAdjacentNode1()) )
        //            {
        //                Routelength = Routelength + CurrentNode.GetDistanceNode1();
        //                CurrentNode = CurrentNode.GetAdjacentNode1();
        //            }
        //            else
        //            {
        //                Routelength = Routelength + CurrentNode.GetDistanceNode2();
        //                CurrentNode = CurrentNode.GetAdjacentNode2();
        //            }
        //        }

        //        if (CurrentNode.NodeName == DestinationNode.NodeName)
        //        {
        //            Visited.Add(CurrentNode);
        //        }

        //        return Visited;
        //    }
    }
}
