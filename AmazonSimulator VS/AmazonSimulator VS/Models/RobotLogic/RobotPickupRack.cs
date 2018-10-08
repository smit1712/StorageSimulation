using System;
using AmazonSimulator;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class RobotPickupRack : IRobotTask
    {
        private Rack rack;
        public RobotPickupRack(Rack r)
        {
            this.rack = r;
        }

        // See IRobotTask for summary
        public void StartTask(Robot r)
        {
            if (TaskCompleted(r))
            {
                return;
            }

            r.PickupRack(rack);
            if (rack.currentNode.busy)
            {
                rack.currentNode.busy = false;
            }
        }

        // See IRobotTask for summary
        public bool TaskCompleted(Robot r)
        {
            if (r.currentRack == rack)
            {
                return true;
            }

            return false;
        }
    }
}
