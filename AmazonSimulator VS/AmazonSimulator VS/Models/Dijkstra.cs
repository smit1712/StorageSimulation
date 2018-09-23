using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmazonSimulator 
{
    public class Dijkstra
    {
        private List<Node> NdeList;
        public Dijkstra(List<Node> Nodelist)
        {
            NdeList = Nodelist;
        }

        public List<Node> GetBestRoute(Node CurrentNode, Node DestinationNode, Node Lastnode)
        {
            Node adjacent1 = CurrentNode.GetAdjacentNode1();
            Node adjacent2 = CurrentNode.GetAdjacentNode2();
            Node adjacent3 = CurrentNode.GetAdjacentNode3();
            List<Node> BestRoute = new List<Node>();
            double Totaldistance1 = GetTotalDistance(adjacent1, DestinationNode);
            double Totaldistance2 = GetTotalDistance(adjacent2, DestinationNode);
            double Totaldistance3 = 123456;
            if (adjacent3 != null)
            {
                 Totaldistance3 = GetTotalDistance(adjacent3, DestinationNode);
            }
            if (CurrentNode == DestinationNode)
            {
                BestRoute.Add(CurrentNode);
                return BestRoute;
            }
            else
            if (Totaldistance1 <= Totaldistance2 && Totaldistance1 < Totaldistance3 && adjacent1 != Lastnode)
            {
                BestRoute.AddRange(GetBestRoute(adjacent1, DestinationNode, CurrentNode));
            }else
            if (Totaldistance2 <= Totaldistance1 && Totaldistance2 < Totaldistance3 && adjacent2 != Lastnode)
            {
                BestRoute.AddRange(GetBestRoute(adjacent2, DestinationNode, CurrentNode));
            }
            else
            if(Totaldistance3 <= Totaldistance2 && Totaldistance3 < Totaldistance1 && adjacent3 != Lastnode && Totaldistance3 != 0)
            {
                BestRoute.AddRange(GetBestRoute(adjacent3, DestinationNode, CurrentNode));
            }
            BestRoute.Add(CurrentNode);
           // BestRoute.Reverse();
            return BestRoute;
        }

        public double GetTotalDistance(Node CurrentNode, Node DestinationNode)
        {
            List<Node> Unvisited = NdeList;
            Unvisited.Remove(CurrentNode);
            List<Node> Visited = new List<Node>();
            

            double Routelength = 0;

            while (CurrentNode.NodeName != DestinationNode.NodeName )
            {
                
                double DistanceN1 = CurrentNode.GetDistanceNode1();
                double DistanceN2 = CurrentNode.GetDistanceNode2();
                double DistanceN3 = CurrentNode.GetDistanceNode3();

                if (CurrentNode.GetAdjacentNode1() == DestinationNode)
                {
                    Visited.Add(CurrentNode);
                    Routelength = Routelength + CurrentNode.GetDistanceNode1();
                    CurrentNode = CurrentNode.GetAdjacentNode1();
                    break;
                }
                if (CurrentNode.GetAdjacentNode2() == DestinationNode)
                {
                    Visited.Add(CurrentNode);
                    Routelength = Routelength + CurrentNode.GetDistanceNode2();
                    CurrentNode = CurrentNode.GetAdjacentNode2();
                    break;
                }
                if (CurrentNode.GetAdjacentNode3() == DestinationNode)
                {
                    Visited.Add(CurrentNode);
                    Routelength = Routelength + CurrentNode.GetDistanceNode3();
                    CurrentNode = CurrentNode.GetAdjacentNode3();
                    break;
                }

                //if (CurrentNode.NodeName == DestinationNode.NodeName)
                //{
                //    Visited.Add(CurrentNode);
                //}

                if (Visited.Count == 0)
                {
                    Visited.Add(CurrentNode);
                    if (DistanceN1 <= DistanceN2 && DistanceN1 <= DistanceN3)
                    {
                        Routelength = Routelength + DistanceN1;                        
                        CurrentNode = CurrentNode.GetAdjacentNode1();
                    }
                    if (DistanceN2 <= DistanceN1 && DistanceN2 <= DistanceN3)
                    {
                        Routelength = Routelength + DistanceN2;                        
                        CurrentNode = CurrentNode.GetAdjacentNode2();
                    }
                    if (DistanceN3 <= DistanceN2 && DistanceN3 <= DistanceN1)
                    {
                        Routelength = Routelength + DistanceN3;
                        CurrentNode = CurrentNode.GetAdjacentNode3();
                    }
                    Visited.Add(CurrentNode);
                }
                else
                {
                    if (Visited.Contains(CurrentNode.GetAdjacentNode1()))
                    {
                        if (DistanceN2 <= DistanceN3)
                        {
                            Visited.Add(CurrentNode);
                            Routelength = Routelength + DistanceN2;
                            CurrentNode = CurrentNode.GetAdjacentNode2();
                        }
                        else
                        {
                            Visited.Add(CurrentNode);
                            Routelength = Routelength + DistanceN3;
                            CurrentNode = CurrentNode.GetAdjacentNode3();
                        }
                    }
                    else
                    if (Visited.Contains(CurrentNode.GetAdjacentNode2()))
                    {
                        if (DistanceN1 <= DistanceN3)
                        {
                            Visited.Add(CurrentNode);
                            Routelength = Routelength + DistanceN1;
                            CurrentNode = CurrentNode.GetAdjacentNode1();
                        }
                        else
                        {
                            Visited.Add(CurrentNode);
                            Routelength = Routelength + DistanceN3;
                            CurrentNode = CurrentNode.GetAdjacentNode3();
                        }
                    }
                    else
                    if (Visited.Contains(CurrentNode.GetAdjacentNode3()))
                    {
                        if (DistanceN1 <= DistanceN2)
                        {
                            Visited.Add(CurrentNode);
                            Routelength = Routelength + DistanceN1;
                            CurrentNode = CurrentNode.GetAdjacentNode1();
                        }
                        else
                        {
                            Visited.Add(CurrentNode);
                            Routelength = Routelength + DistanceN2;
                            CurrentNode = CurrentNode.GetAdjacentNode2();
                        }
                    }

                    return Routelength;
                }



            }
            if(CurrentNode.NodeName == DestinationNode.NodeName)
            {

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
