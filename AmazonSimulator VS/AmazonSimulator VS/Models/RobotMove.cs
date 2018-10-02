using System;
using AmazonSimulator;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class RobotMove : IRobotTask
    {
        private List<Node> path = new List<Node>();

        public RobotMove(Node[] nodesInPath)
        {
            foreach (Node node in nodesInPath) {
                this.path.Add(node);
            }
        }

        public void StartTask(Robot r)
        {
            if (TaskCompleted(r))
            {
                return;
            }

            r.currentNode = path[0];
            MoveOverPath(r);
        }

        public bool TaskCompleted(Robot r)
        {
            if (r.x == path[0].x && r.y == path[0].y && r.z == path[0].z)
            {
                path.RemoveAt(0);
            }

            if (path.Count == 0)
            {
                return true;
            }

            return false;
        }

        double newX, newY, newZ = 0;
        public void MoveOverPath(Robot r)
        {
            newX = r.x;
            newY = r.y;
            newZ = r.z;

            // Adjust x as
            if (r.x < path[0].x)
            {
                newX += .2;
            }
            else if (r.x > path[0].x)
            {
                newX -= .2;
            }

            if (Math.Abs(r.x - path[0].x) < .2)
            {
                newX = path[0].x;
            }

            // Adjust y as
            if (r.y < path[0].y)
            {
                newY += .2;
            }
            else if (r.y > path[0].y)
            {
                newY -= .2;
            }

            if (Math.Abs(r.y - path[0].y) < .2)
            {
                newY = path[0].y;
            }

            // Adjust z as
            if (r.z < path[0].z)
            {
                newZ += .2;
            }
            else if (r.z > path[0].z)
            {
                newZ -= .2;
            }

            if (Math.Abs(r.z - path[0].z) < .2)
            {
                newZ = path[0].z;
            }

            // Move robot to new position
            r.Move(newX, newY, newZ);
            if (r.currentRack != null)
            {
                r.currentRack.Move(newX, newY, newZ);
            }
        }
    }
}
