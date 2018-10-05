using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public interface IRobotTask
    {
        void StartTask(Robot r);
        bool TaskCompleted(Robot r);
    }
}
