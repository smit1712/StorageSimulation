using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using AmazonSimulator;

namespace Models
{
    public class Robot : Model3D, IUpdatable
    {
        public List<IRobotTask> tasks = new List<IRobotTask>();
        public Rack currentRack = null;
        public Node currentNode;
        private double hover = 0;
        private double starty;

        public Robot(double x, double y, double z, double rotationX, double rotationY, double rotationZ, Node node) : base(x, y, z, rotationX, rotationY, rotationZ)
        {
            this.type = "robot";
            this.currentNode = node;
            starty = y;
        }

        /// <summary>
        /// Adds a new IRobotTask to tasks
        /// </summary>
        /// <param name="task"></param>
        public void AddTask(IRobotTask task)
        {
            tasks.Add(task);
        }

        /// <summary>
        /// Whenever tasks > 0, execute first task
        /// </summary>
        /// <param name="tick"></param>
        /// <returns></returns>
        public override bool Update(int tick)
        {
            if (tasks.Count > 0)
            {
                if (tasks.First().TaskCompleted(this))
                {
                    tasks.RemoveAt(0);
                }
                else
                {
                    tasks.First().StartTask(this);
                }
            }

            return true;
        }
        
        /// <summary>
        /// Set rack to robot
        /// </summary>
        /// <param name="rack"></param>
        public void PickupRack(Rack rack)
        {
            currentRack = rack;
        }

        /// <summary>
        /// DIsarm rack from robot
        /// </summary>
        public void DropRack()
        {
            currentRack.currentNode = currentNode;

            currentRack.Move(this.x, 0.2, this.z - 2);
            currentRack = null;
        }

        /// <summary>
        /// Adds smooth hover to robot movement
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Hover(double x, double y, double z)
        {
            if (hover > 360)
            {
                hover = 0;
            }

            y = starty + Math.Sin(hover) * 0.5;
            hover += 0.1;
            Move(x, y, z);
        }
    }
}