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
        private Node homeNode;
        private Dijkstra dijkstra;
        // All models
        private List<Model3D> worldObjects = new List<Model3D>();
        // Racks
        
        
        private List<Rack> newRacks = new List<Rack>();
        private List<Rack> storedRacks = new List<Rack>();
        private List<Rack> emptyRacks = new List<Rack>();
        // Used for randomness in spawning/getting racks
        private Random random = new Random();
        // Clients
        private List<IObserver<Command>> observers = new List<IObserver<Command>>();

        public World() {
            CreateTransport(-1.0, 0.4, -10);

            // Setup all nodes
            NodeCreator nodeCreator = new NodeCreator(30, 30);
            this.nodeList = nodeCreator.GetNodeList();
            this.homeNode = nodeList[27];

            CreateRobot(10, 0.15, 10);
            Sun Sun = new Sun(0, 0, 0, 0, 0, 0, 100,500);
            worldObjects.Add(Sun);

            CreateRobot(1, 0.15, 10, this.homeNode);
            CreateRobot(1, 0.15, 10, this.homeNode);
            CreateRobot(1, 0.15, 10, this.homeNode);
            CreateRobot(1, 0.15, 10, this.homeNode);
            CreateRobot(1, 0.15, 10, this.homeNode);
            CreateRobot(1, 0.15, 10, this.homeNode);

            // Set list that tracks wether a node 
            for (int i = nodeList.Count; i > 0; i--)
            {
                if (i != 1 && i % 7 == 1)
                {
                    rackPlacedList.Add(true);
                } else
                {
                    rackPlacedList.Add(false);
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
            List<Node> route = new List<Node>();

            // Try out node route
            //Robot r = CreateRobot(10, 0.15, 10, this.homeNode);
            //route = this.dijkstra.GetBestRoute(nodeList[5], nodeList[27]);
            //r.AddTask(new RobotMove(route.ToArray()));

            //this.newRacks.Add(CreateRack(3, 0.15, 12.5, this.homeNode));
            //CreateRack(10, 0.15, 10);
            //CreateRack(15, 0.15, 15);
            //CreateRack(20, 0.15, 20);

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
            Transport t = new Transport(x, y, z, 0, 0, 0);
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
                            if (r.currentNode == homeNode && r.currentRack == null)
                            {
                                if (storedRacks.Count > 5) // Search for rack and place it at the home node
                                {
                                    Rack chosenRack = null;
                                    bool chooseRack = true;

                                    int numb = 0;
                                    while (chooseRack)
                                    {
                                        numb = random.Next(0, storedRacks.Count);
                                        bool getRack = rackPlacedList[numb];

                                        if (getRack)    // Chosen position contains a rack
                                        {
                                            chooseRack = false;
                                            rackPlacedList[numb] = false;
                                            chosenRack = storedRacks[numb];
                                        }
                                    }

                                    // Ride robot to position, pickup rack and drop at homenode
                                    r.AddTask(new RobotMove(this.dijkstra.GetBestRoute(r.currentNode, chosenRack.currentNode).ToArray()));
                                    r.AddTask(new RobotPickupRack(chosenRack));
                                    r.AddTask(new RobotMove(this.dijkstra.GetBestRoute(chosenRack.currentNode, this.homeNode).ToArray()));
                                    r.AddTask(new RobotDropRack());

                                    // Update storage
                                    int test = storedRacks.IndexOf(chosenRack);
                                    emptyRacks.Add(chosenRack);
                                    storedRacks.RemoveAt(test);
                                }
                                else if (newRacks.Count > 0)    // Pickup rack, drop in storage and return
                                {
                                    for (int count = 0; count < nodeList.Count; count++)
                                    {
                                        Console.WriteLine(count + ": " + rackPlacedList[count]);
                                    }

                                    Node placeRackNode = null;
                                    for (int count = 0; count < nodeList.Count; count++)    // Get avaible place in storage
                                    {
                                        if (rackPlacedList[count] == false)
                                        {
                                            placeRackNode = nodeList[count];
                                            rackPlacedList[count] = true;
                                            count = nodeList.Count;
                                        }
                                    }

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

                        } else if (m3d is Sun)
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