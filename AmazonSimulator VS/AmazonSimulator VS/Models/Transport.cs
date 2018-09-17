using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class Transport : Model3D, IUpdatable
{
        Stack<Rack> Rackstack = new Stack<Rack>();
        double tempz;
        public Transport(double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(x, y, z, rotationX, rotationY, rotationZ)
        {
            this.type = "transport";
            this.guid = Guid.NewGuid();
            tempz = z;
        }
        public virtual bool Update(int tick)
        {
            if (needsUpdate)
            {
                Delivery();
                return true;
            }
            return false;
        }

        public void Delivery()
        {
            if (z > 10.5 && z < 10.70)
            {
                System.Threading.Thread.Sleep(5000);
                Move(x, y, 11.1);
            }
            if (z > 30)
            {
                tempz = -10;
                Move(x, y, -10);
            }
            else
            {
                tempz = tempz + 0.15;
                Move(x, y, tempz);
            }
        }
        public void GetRack(Rack Getrack)
        {
            Rackstack.Push(Getrack);
            needsUpdate = true;
        }
        public Rack DropRack()
        {
            needsUpdate = true;
            return Rackstack.Pop();
        }
    }
}
