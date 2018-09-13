using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class Transport : Model3D, IUpdatable
{
        Stack<Rack> Rackstack = new Stack<Rack>();
        public Transport(double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(x, y, z, rotationX, rotationY, rotationZ)
        {
            this.type = "transport";
            this.guid = Guid.NewGuid();
        }
        public virtual bool Update(int tick)
        {
            if (needsUpdate)
            {
                needsUpdate = false;
                return true;
            }
            return false;
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
