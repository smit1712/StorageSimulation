using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using AmazonSimulator;

namespace Models {
    public class Robot : Model3D, IUpdatable
    {
        private List<IRobotTask> tasks = new List<IRobotTask>();
        public Rack currentRack = null;
        public Node currentNode;

        //public bool hasRack = false;

        public Robot(double x, double y, double z, double rotationX, double rotationY, double rotationZ, Node node) : base(x,y,z,rotationX,rotationY,rotationZ)
        {
            this.type = "robot";
            this.currentNode = node;
        }

        public void AddTask(IRobotTask task)
        {
            tasks.Add(task);
        }

        public override bool Update(int tick)
        {
            if (tasks.Count > 0)
            {
                if (tasks.First().TaskCompleted(this))
                {
                    tasks.RemoveAt(0);
                } else
                {
                    tasks.First().StartTask(this);
                }
            }

            return true;
        }

        public void PickupRack(Rack rack)
        {
            currentRack = rack;
        }

        public void DropRack()
        {
            currentRack.currentNode = currentNode;
            currentRack.Move(this.x, this.y, this.z - 2);
            currentRack = null;
        }

    }
}