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
            int rowsize = Convert.ToInt32((maxX +5) / 5 );
            foreach (Node N in Nlist)
            {
                if (nodecounter == 0)
                {
                    N.AddAdjacentNode1(Nlist[7]);
                    N.AddAdjacentNode2(Nlist[1]);
                    
                } else if (nodecounter +1  == Nlist.Count)
                {
                    N.AddAdjacentNode1(Nlist[nodecounter - 1]);
                    N.AddAdjacentNode2(Nlist[nodecounter - 1]);
                    
                }
                else
                {
                    N.AddAdjacentNode1(Nlist[nodecounter - 1]);
                    N.AddAdjacentNode2(Nlist[nodecounter + 1]);
                    
                }
                if ((nodecounter +1 == rowsize))
                {
                    N.type = "adj";
                    N.AddAdjacentNode1(Nlist[nodecounter - 1]);
                    N.AddAdjacentNode2(Nlist[nodecounter + rowsize]);
                }
                if ((nodecounter % rowsize) == 0)
                {

                    if ((nodecounter - rowsize) >= 0)
                    {
                        N.AddAdjacentNode1(Nlist[nodecounter - rowsize]);
                    }
                    if ((nodecounter + rowsize) < Nlist.Count)
                    {
                        N.AddAdjacentNode2(Nlist[nodecounter + rowsize]);
                    }
                    
                    N.AddAdjacentNode3(Nlist[nodecounter +1]);                    
                }
                if ((nodecounter % rowsize) == (rowsize-1))
                {
                    if ((nodecounter - rowsize) >= 0)
                    {
                        N.AddAdjacentNode3(Nlist[nodecounter - rowsize]);
                    }
                    if ((nodecounter + rowsize) < Nlist.Count)
                    {
                        N.AddAdjacentNode2(Nlist[nodecounter + rowsize]);
                    }
                }
                if (N.GetAdjacentNode3() != null)
                {
                    
                    N.type = "adj";
                }

                nodecounter++;
            }

            return Nlist;
        }


}
}
