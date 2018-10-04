using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AmazonSimulator;

namespace Models
{
	public class Rack : Model3D, IUpdatable
    {
        public Node currentNode;

        public Rack(double x, double y, double z, double rotationX, double rotationY, double rotationZ, Node node) : base(x, y, z, rotationX, rotationY, rotationZ)
        {
            this.type = "rack";
            this.currentNode = node;
        }
    }
}
