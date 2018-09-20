using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmazonSimulator
{
    public class Node
    {
        public double _x = 0;
        public double _y = 0;
        public double _z = 0;

        Node AdjacentNode1;
        Node AdjacentNode2;

        public string NodeName;
        public Node(double x, double y, double z,string NName)
        {
            NodeName = NName;
            _x = x;
            _y = y;
            _z = z;
        }
        public double Getx()
        {
            return _x;
        }
        public double Gety()
        {
            return _y;
        }
        public double Getz()
        {
            return _z;
        }
        public void AddAdjacentNode1(Node Node1)
        {
            AdjacentNode1 = Node1;
            
        }
        public void AddAdjacentNode2(Node Node2)
        {
            AdjacentNode2 = Node2;
        }

        public Node GetAdjacentNode1()
        {
            return AdjacentNode1;
        }
        public Node GetAdjacentNode2()
        {
            return AdjacentNode2;
        }

        public double GetDistanceNode1()
        {            
            double Dx = Math.Pow((_x - AdjacentNode1.Getx()), 2);
            double Dy = Math.Pow((_y - AdjacentNode1.Gety()), 2);
            double Dz = Math.Pow((_z - AdjacentNode1.Getz()), 2);

            double result = (Dx + Dy + Dz) * 0.5;

            return result;
        }
        public double GetDistanceNode2()
        {
            double Dx = Math.Pow((_x - AdjacentNode2.Getx()), 2);
            double Dy = Math.Pow((_y - AdjacentNode2.Gety()), 2);
            double Dz = Math.Pow((_z - AdjacentNode2.Getz()), 2);

            double result = (Dx + Dy + Dz) * 0.5;

            return result;
        }
        public double GetDistanceTo(Node DistanceNode)
        {
            double Dx = Math.Pow((_x - DistanceNode.Getx()), 2);
            double Dy = Math.Pow((_y - DistanceNode.Gety()), 2);
            double Dz = Math.Pow((_z - DistanceNode.Getz()), 2);

            double result = (Dx + Dy + Dz) * 0.5;

            return result;
        }



    }
}
