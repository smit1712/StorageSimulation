using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmazonSimulator 
{
    public class Sun : Model3D, IUpdatable
    {
        double Angle;
        double Radius;
        double originx;
        double originy;
        double originz;
        public Sun(double x, double y, double z, double rotationX, double rotationY, double rotationZ, double angle, double radius) : base(x, y, z, rotationX, rotationY, rotationZ)
        {
            Angle = angle;
            Radius = radius;
            this.type = "sun";
            Circle();
            originx = x;
            originy = y;
            originz = z;
        }
        public override bool Update(int tick)
        {
            if (needsUpdate)
            {
                needsUpdate = false;
                return true;
            }
            Circle();
            return false;
        }
        private void Circle()
        {
            if (Angle > 360)
            {
                Angle = 0;
            }

            double circleX = originx + Math.Sin(Angle) * Radius;
            double circleY = originy + Math.Cos(Angle) * Radius;
           // double circleZ = x + Math.Sin(Angle) * Radius;
            Move(circleX, circleY, 0);
            needsUpdate = true;
            Angle += 0.001;
        }
    }

}

