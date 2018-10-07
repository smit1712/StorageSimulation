using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public interface IRobotTask
    {
        /// <summary>
        /// Starts up a task
        /// </summary>
        /// <param name="r"></param>
        void StartTask(Robot r);

        /// <summary>
        /// Checks wether a task is completed
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        bool TaskCompleted(Robot r);
    }
}
