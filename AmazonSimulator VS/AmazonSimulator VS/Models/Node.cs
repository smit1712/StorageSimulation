using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmazonSimulator 
{
    public class Node : Model3D, IUpdatable
    {
        Node AdjacentNode1;
        Node AdjacentNode2;
        Node AdjacentNode3;

        Node PreviousNode;

        private double DistanceFromOrigin;
        public string NodeName;
        public Node(double x, double y, double z,string NName) : base(x, y, z, 0, 0, 0) 
        {
            this.type = "node";
            this.guid = Guid.NewGuid();

            NodeName = NName;

        }
        public double Getx()
        {
            return x;
        }
        public double Gety()
        {
            return y;
        }
        public double Getz()
        {
            return z;
        }
        public void AddAdjacentNode1(Node Node1)
        {
            AdjacentNode1 = Node1;
            
        }
        public void AddAdjacentNode2(Node Node2)
        {
            AdjacentNode2 = Node2;
        }
        public void AddAdjacentNode3(Node Node3)
        {
            AdjacentNode3 = Node3;
        }
        public Node GetAdjacentNode1()
        {
            return AdjacentNode1;
        }
        public Node GetAdjacentNode2()
        {
            return AdjacentNode2;
        }
        public Node GetAdjacentNode3()
        {
            return AdjacentNode3;
        }

        public double GetDistanceNode1()
        {            
            double Dx = Math.Pow((x - AdjacentNode1.Getx()), 2);
            double Dy = Math.Pow((y - AdjacentNode1.Gety()), 2);
            double Dz = Math.Pow((z - AdjacentNode1.Getz()), 2);

            double result = (Dx + Dy + Dz) * 0.5;

            return result;
        }
        public double GetDistanceNode2()
        {
            double Dx = Math.Pow((x - AdjacentNode2.Getx()), 2);
            double Dy = Math.Pow((y - AdjacentNode2.Gety()), 2);
            double Dz = Math.Pow((z - AdjacentNode2.Getz()), 2);

            double result = (Dx + Dy + Dz) * 0.5;

            return result;
        }
        public double GetDistanceNode3()
        {
            if (AdjacentNode3 != null)
            {
                double Dx = Math.Pow((x - AdjacentNode3.Getx()), 2);
                double Dy = Math.Pow((y - AdjacentNode3.Gety()), 2);
                double Dz = Math.Pow((z - AdjacentNode3.Getz()), 2);

                double result = (Dx + Dy + Dz) * 0.5;

                return result;
            }
            else return 12345;
        }
        public double GetDistanceTo(Node DistanceNode)
        {
            double Dx = Math.Pow((x - DistanceNode.Getx()), 2);
            double Dy = Math.Pow((y - DistanceNode.Gety()), 2);
            double Dz = Math.Pow((z - DistanceNode.Getz()), 2);

            double result = (Dx + Dy + Dz) * 0.5;

            return result;
        }
        public void SetDistanceFromOrigin(double Distance)
        {
            DistanceFromOrigin = Distance;
        }
        public double GetDistanceFromOrigin()
        {
            return DistanceFromOrigin;
        }
        public void SetPreviousNode(Node node)
        {
            PreviousNode = node;
        }
        public Node GetPreviousNode()
        {
            return PreviousNode;
        }


    }
}
