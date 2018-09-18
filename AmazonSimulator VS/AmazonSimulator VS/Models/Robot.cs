using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Models {
    public class Robot : Model3D, IUpdatable
    {
        public Rack currentRack;

        public bool hasRack = false;
        public bool reachedDestiny = false;

        public Robot(double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(x,y,z,rotationX,rotationY,rotationZ)
        {
            this.type = "robot";
            this.guid = Guid.NewGuid();
        }
        
        bool forward = true;
        public void SearchRack()
        {
            double destX;
            
            if (forward) {
                destX = x + 1;
            } else {
                destX = x - 1;
            }

            Move(destX, y, z);
            if (currentRack != null)
            {
                currentRack.Move(destX, y + 2, z);
            }

            if (x < 1 || x > 20)
            {
                forward = !forward;
                reachedDestiny = true;
            } else
            {
                reachedDestiny = false;
            }
        }

        public void PickupRack(Rack rack)
        {
            currentRack = rack;
            hasRack = true;
        }

        public void DropRack()
        {
            currentRack = null;
            hasRack = false;
        }
    }
}