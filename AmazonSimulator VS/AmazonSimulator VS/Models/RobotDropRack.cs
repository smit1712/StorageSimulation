using System;
using AmazonSimulator;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class RobotDropRack : IRobotTask
    {
        public RobotDropRack()
        {
            
        }

        public void StartTask(Robot r)
        {
            if (TaskCompleted(r))
            {
                return;
            }

            r.DropRack();
        }

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
