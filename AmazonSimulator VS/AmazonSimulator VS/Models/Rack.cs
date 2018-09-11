using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class Rack : _3DModel,IUpdatable
{
        public Rack(double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(x, y, z, rotationX, rotationY, rotationZ)
        {
            this.type = "Rack";
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
    }
}
