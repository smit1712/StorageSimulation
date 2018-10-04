using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Models
{
	public class Transport : Model3D, IUpdatable
    {
        public bool reachedLoader = false;
        public bool createdRacks = false;
        private double speed = 1;

        public Transport(double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(x, y, z, rotationX, rotationY, rotationZ)
        {
            this.type = "transport";
        }

        bool countedTick = false;
        int countTick = 0;
        double newZ = 0;
        public void UpdatePosition()
        {
            if (countTick > 0)
            {
                countTick++;

                if (countTick > 50)
                {
                    countTick = 0;
                    reachedLoader = false;
                    createdRacks = false;
                }
                needsUpdate = true;
                return;
            }
            if (z >= 0 && z < 12.5 )
            {
                speed += -1.5;
            }

            if (z >= 12.5 && z < 30 && !countedTick)
            {
                countTick = 1;
                reachedLoader = true;
                countedTick = true;
                needsUpdate = true;
                return;
                // Truck stands still for * amount of ticks
            }
            else if (z >= 150)
            {
                if (countedTick)
                {
                    countTick = 1;
                    visible = false;
                    countedTick = false;
                    // Truck stands still for * amount of ticks
                }
                else
                {
                    speed = 1;
                    newZ = -100;
                    visible = true;
                    // Reset truck to start position
                }
            }
            else
            {
                newZ = z + 0.15 * speed;
                speed += 0.2;
            }

            Move(x, y, newZ);
        }

        private void CreateRacks()
        {

        }
    }
}
