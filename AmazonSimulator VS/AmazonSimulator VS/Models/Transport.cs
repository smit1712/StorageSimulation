using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Models
{
	public class Transport : Model3D, IUpdatable
{
        Stack<Rack> Rackstack = new Stack<Rack>();


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
                Console.WriteLine(countTick);
                if (countTick > 50)
                {
                    countTick = 0;
                }
                needsUpdate = true;
                return;
            }
            
            if (z >= 10.5 && z < 30 && !countedTick)
            {
                countTick = 1;
                countedTick = true;
                needsUpdate = true;
                return;
                // Truck stands still for * amount of ticks
            }
            else if (z >= 30)
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
                    newZ = 0;
                    visible = true;
                    // Reset truck to start position
                }
            }
            else
            {
                newZ = z + 0.15;
            }

            Move(x, y, newZ);
        }
    }
}
