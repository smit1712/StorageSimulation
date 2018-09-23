using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmazonSimulator 
{
    public class NodeCreator
{
        private double maxX,  maxZ;
        List<Node> Nlist = new List<Node>();
        int newnodeID = 0;
        public NodeCreator(double MaxX, double MaxZ)
        {
            maxX = MaxX;
            
            maxZ = MaxZ;
        }        
            //A.AddAdjacentNode1(E);
            //A.AddAdjacentNode2(B);

            //B.AddAdjacentNode1(A);
            //B.AddAdjacentNode2(C);

            //C.AddAdjacentNode1(B);
            //C.AddAdjacentNode2(D);

            //D.AddAdjacentNode1(C);
            //D.AddAdjacentNode2(E);

            //E.AddAdjacentNode1(D);
            //E.AddAdjacentNode2(A);
        public List<Node> GetNodeList()
        {
            double x = maxX;
            double z = maxZ;
            while ((z - 5) >= -5)
            {
                while ((x -5) >= -5)
                {
                    Node node = new Node(x, 0, z, newnodeID.ToString());
                    Nlist.Add(node);
                    x = x - 5;
                    newnodeID++;
                    
                }
                
                z = z - 5;
                x = maxX;

            }
            int nodecounter = 0;
            foreach (Node N in Nlist)
            {
                if (nodecounter == 0)
                {
                    N.AddAdjacentNode1(Nlist.Last());
                    N.AddAdjacentNode2(Nlist[1]);
                    
                } else if (nodecounter +1  == Nlist.Count)
                {
                    N.AddAdjacentNode1(Nlist[nodecounter - 1]);
                    N.AddAdjacentNode2(Nlist.First());
                    
                }
                else
                {
                    N.AddAdjacentNode1(Nlist[nodecounter - 1]);
                    N.AddAdjacentNode2(Nlist[nodecounter + 1]);
                    
                }
                if (N.z == 0)
                {
                    N.AddAdjacentNode3(Nlist[nodecounter-7]);
                }
                if(N.z == maxZ)
                {
                    N.AddAdjacentNode3(Nlist[nodecounter + 7]);
                }
                //if (N.z == 0 || N.z == maxZ)
                //{
                //   double tempz = N.z;
                //   int tempint = Convert.ToInt32(tempz / (maxz + tempz));
                //    N.AddAdjacentNode3(Nlist[tempint]);
                //}
                nodecounter++;
            }

            return Nlist;
        }


}
}
