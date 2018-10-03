using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using System.Threading;
using AmazonSimulator;

namespace Models {
    public class World : IObservable<Command>, IUpdatable
    {
        // Nodes en Dijkstra algorithm
        private List<Node> nodeList = new List<Node>();
        private List<bool> rackPlacedList = new List<bool>();
        private List<int> unavailablePlaces = new List<int>();
        private Node homeNode;
        private Dijkstra dijkstra;
        // All models
        private List<Model3D> worldObjects = new List<Model3D>();
        // Racks
        
        
        private List<Rack> newRacks = new List<Rack>();
        private List<Rack> storedRacks = new List<Rack>();
        private List<Rack> emptyRacks = new List<Rack>();
        // Used for randomness in spawning/getting rackss
        private Random random = new Random();
        // Clients
        private List<IObserver<Command>> observers = new List<IObserver<Command>>();

        public World() {
            CreateTransport(0,5.0,0);

            // Setup all nodes
            NodeCreator nodeCreator = new NodeCreator(30, 30);
            this.nodeList = nodeCreator.GetNodeList();
            this.homeNode = nodeList[27];

            Sun Sun = new Sun(0, 0, 0, 0, 0, 0, 100,500);
            worldObjects.Add(Sun);

            for (int i = 10; i >=0; i--)
            {
                CreateRobot(1, 3.15, 10, this.homeNode);
            }

            // Set list that tracks wether a node 
            foreach (Node n in nodeList)
            {
                if (n.type == "adj")
                {
                    this.rackPlacedList.Add(true);
                    this.unavailablePlaces.Add(Convert.ToInt32(n.NodeName));
                }
                else
                {
                    this.rackPlacedList.Add(false);
                }
            }

            List<Node> CornerList = new List<Node>();    
            foreach(Node n in nodeList)
            {
                if(n.GetAdjacentNode3() != null)
                {
                    CornerList.Add(n);
                }
            }

            List<Node> adjlist = new List<Node>();
            foreach (Node n in this.nodeList)
            {
                if (n.GetAdjacentNode3() != null)
                {
                    adjlist.Add(n.GetAdjacentNode3());
                }
            }

            this.dijkstra = new Dijkstra(nodeList);

            //worldObjects.AddRange(CornerList);
            //worldObjects.AddRange(adjlist);
            //worldObjects.AddRange(NodeList);
        }


        private Robot CreateRobot(double x, double y, double z, Node node) {
            Robot r = new Robot(x, y, z, 0, 0, 0, node);
            worldObjects.Add(r);
            return r;
        }

        private Rack CreateRack(double x, double y, double z, Node node)
        {
            Rack r = new Rack(x, y, z, 0, 0, 0, node);
            worldObjects.Add(r);
            return r;
        }

        private Transport CreateTransport(double x, double y, double z)
        {
            Transport t = new Transport(x, y, z, 0, 1.5, 0);
            worldObjects.Add(t);
            return t;
        }

        public bool Update(int tick)
        {
            for (int i = worldObjects.Count-1; i >= 0; i--)
            { 
                if (worldObjects[i] is IUpdatable)
                {
                    bool needsCommand = ((IUpdatable)worldObjects[i]).Update(tick);

                    if (needsCommand)
                    {
                        if  (worldObjects[i] is Robot)
                        {
                            Robot r = (Robot)worldObjects[i];
                            if (r.currentNode == homeNode && r.currentRack == null && r.tasks.Count == 0)
                            {
                                if (storedRacks.Count > 20) // Search for rack and place it at the home node
                                {
                                    int randomRackInt = 0;
                                    Rack randomRack = null;
                                    int randomRackNodeName = 0;

                                    bool checkRack = false;
                                    while (!checkRack)
                                    {
                                        randomRackInt = random.Next(storedRacks.Count);
                                        randomRack = storedRacks[randomRackInt];
                                        randomRackNodeName = Convert.ToInt32(randomRack.currentNode.NodeName);

                                        if (randomRackNodeName !=  Convert.ToInt32(this.homeNode.NodeName))
                                        {
                                            checkRack = true;
                                        }
                                    }

                                    rackPlacedList[randomRackNodeName] = false;

                                    // Ride robot to position, pickup rack and drop at homenode
                                    r.AddTask(new RobotMove(this.dijkstra.GetBestRoute(r.currentNode, randomRack.currentNode).ToArray()));
                                    r.AddTask(new RobotPickupRack(randomRack));
                                    r.AddTask(new RobotMove(this.dijkstra.GetBestRoute(randomRack.currentNode, this.homeNode).ToArray()));
                                    r.AddTask(new RobotDropRack());

                                    randomRack.currentNode.busy = true;
                                    emptyRacks.Add(randomRack);
                                    storedRacks.RemoveAt(storedRacks.IndexOf(randomRack));
                                }
                                else if (newRacks.Count > 0)    // Pickup rack, drop in storage and return
                                {
                                    Node placeRackNode = null;
                                    for (int count = 0; count < nodeList.Count; count++)    // Get avaible place in storage
                                    {
                                        if (rackPlacedList[count] == false)
                                        {
                                            placeRackNode = nodeList[count];
                                            if (!placeRackNode.busy)
                                            {
                                                rackPlacedList[count] = true;
                                                count = nodeList.Count;
                                            }
                                        }
                                    }

                                    int f = 0;
                                    foreach (bool b in rackPlacedList)
                                    {
                                        if (f < 10)
                                        {
                                            if (b && !this.unavailablePlaces.Contains(f))
                                            {
                                                Console.WriteLine(f + " : " + b);
                                            }
                                            f++;
                                        }
                                    }
                                    Console.WriteLine("Placing a new rack on node: " + placeRackNode.NodeName);
                                    // Pickup rack, Ride robot to position and drop rack
                                    r.AddTask(new RobotPickupRack(newRacks[0]));
                                    r.AddTask(new RobotMove(this.dijkstra.GetBestRoute(r.currentNode, placeRackNode).ToArray()));
                                    r.AddTask(new RobotDropRack());
                                    r.AddTask(new RobotMove(this.dijkstra.GetBestRoute(placeRackNode, this.homeNode).ToArray()));

                                    // Update storage
                                    storedRacks.Add(newRacks[0]);
                                    newRacks.RemoveAt(0);
                                }
                            }
                        } else if (worldObjects[i] is Transport)
                        {
                            Transport t = (Transport)worldObjects[i];
                            t.UpdatePosition();
                            
                            if (t.reachedLoader && !t.createdRacks)
                            {
                                foreach (Rack r in emptyRacks)
                                {
                                    if (r.currentNode == this.homeNode)
                                    {
                                        r.visible = false;
                                    }
                                }

                                t.createdRacks = true;

                                int generatedRacks = 0;

                                if (storedRacks.Count <= 30)
                                {
                                    generatedRacks = random.Next(3, 5);
                                } else if (storedRacks.Count > 30 && storedRacks.Count < 40)
                                {
                                    generatedRacks = random.Next(1, 5);
                                } else if (storedRacks.Count >= 40)
                                {
                                    generatedRacks = 0;
                                }
                                
                                // Spaw new racks when truck has reached the loader
                                for (int a = generatedRacks; a > 0; a--)
                                {
                                    newRacks.Add(CreateRack(homeNode.x + 1, homeNode.y + 0.15, homeNode.z, homeNode));
                                }
                                Console.Write("New racks: " + newRacks.Count);
                                Console.WriteLine(" Stored racks: " + storedRacks.Count);
                            }
                        } else if (worldObjects[i] is Rack)
                        {

                        } else if (worldObjects[i] is Node)
                        {

                        }
                        else if (worldObjects[i] is Sun)
                        {
                            
                        }

                        // Send Model through socket
                        SendCommandToObservers(new UpdateModel3DCommand(worldObjects[i]));
                    }
                }
            }

            return true;
        }

        public IDisposable Subscribe(IObserver<Command> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);

                SendCreationCommandsToObserver(observer);
            }
            return new Unsubscriber<Command>(observers, observer);
        }

        private void SendCommandToObservers(Command c)
        {
            for (int i = 0; i < this.observers.Count; i++)
            {
                this.observers[i].OnNext(c);
            }
        }

        private void SendCreationCommandsToObserver(IObserver<Command> obs)
        {
            foreach (Model3D m3d in worldObjects)
            {
                obs.OnNext(new UpdateModel3DCommand(m3d));
            }
        }
    }

    internal class Unsubscriber<Command> : IDisposable
    {
        private List<IObserver<Command>> _observers;
        private IObserver<Command> _observer;

        internal Unsubscriber(List<IObserver<Command>> observers, IObserver<Command> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose() 
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}