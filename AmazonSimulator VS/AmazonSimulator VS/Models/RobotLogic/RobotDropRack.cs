using System;
using AmazonSimulator;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class RobotDropRack : IRobotTask
    {
        // See IRobotTask for summary
        public void StartTask(Robot r)
        {
            if (TaskCompleted(r))
            {
                return;
            }

            r.DropRack();
        }

        // See IRobotTask for summary
        public bool TaskCompleted(Robot r)
        {
            if (r.currentRack == null)
            {
                return true;
            }

            return false;
        }
    }
}
