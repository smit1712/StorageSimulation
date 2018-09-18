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
        public List<Node> GetRoute(Node CurrentNode, Node DestinationNode)
        {
            List<Node> Unvisited = NodeList;
            Unvisited.Remove(CurrentNode);           
            List<Node> Visited = new List<Node>();           
            List<Node> Route = new List<Node>();

            double Routelength = 0;

            while (CurrentNode.NodeName != DestinationNode.NodeName)
            {
                Visited.Add(CurrentNode);

                if(CurrentNode.GetAdjacentNode1() == DestinationNode)
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
                if (CurrentNode.GetDistanceNode1() < CurrentNode.GetDistanceNode2() && !Visited.Contains(CurrentNode.GetAdjacentNode1()) )
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

            return Visited;
        }
    }
}
