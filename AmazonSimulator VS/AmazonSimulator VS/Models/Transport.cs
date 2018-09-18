﻿using System;
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
            this.guid = Guid.NewGuid();
        }
    }
}
