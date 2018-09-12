using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Models {
    public class Robot : _3DModel , IUpdatable
    {
        Rack CurrentRack;
        bool HasRack = false;
        public Robot(double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(x,y,z,rotationX,rotationY,rotationZ)
        {
            this.type = "Robot";
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
            if (HasRack == false)
            {
                HasRack = true;
                CurrentRack = Getrack;
                needsUpdate = true;
            }
        }
        public Rack DropRack()
        {
            HasRack = false;
            needsUpdate = true;
            return CurrentRack;
        }
    }
}