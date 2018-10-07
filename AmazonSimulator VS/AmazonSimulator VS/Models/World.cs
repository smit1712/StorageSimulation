using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using System.Threading;
using AmazonSimulator;

namespace Models
{
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

        public World()
        {

            // Setup all nodes
            NodeCreator nodeCreator = new NodeCreator(30, 30);
            this.nodeList = nodeCreator.GetNodeList();
            this.homeNode = nodeList[27];

            // Add a Sun
            Sun Sun = new Sun(0, 0, 0, 0, 0, 0, 100, 500);
            worldObjects.Add(Sun);

            // Init some robots to work with
            int RobotCount = 10;
            for (int i = RobotCount; i >= 0; i--)
            {
                CreateRobot(this.homeNode.x, 3.15, this.homeNode.z, this.homeNode);
            }

            // Set list that tracks wether a rack can be placed in a node
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

            // Init a transport to deliver and get racks
            CreateTransport(0, 5.0, -100);

            // Setup dijkstra method to calculate routes
            this.dijkstra = new Dijkstra(nodeList);
        }

        /// <summary>
        /// Adds new Robot object to worldobjects
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="node"></param>
        /// <returns>Robot</returns>
        private Robot CreateRobot(double x, double y, double z, Node node)
        {
            Robot r = new Robot(x, y, z, 0, 0, 0, node);
            worldObjects.Add(r);
            return r;
        }

        /// <summary>
        /// Adds new Rack object to worldobjects
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="node"></param>
        /// <returns>Rack</returns>
        private Rack CreateRack(double x, double y, double z, Node node)
        {
            Rack r = new Rack(x, y, z, 0, 0, 0, node);
            worldObjects.Add(r);
            return r;
        }

        /// <summary>
        /// Adds new Transport object to worldobjects
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns>Transport</returns>
        private Transport CreateTransport(double x, double y, double z)
        {
            Transport t = new Transport(x, y, z, 0, 1.5, 0);
            worldObjects.Add(t);
            return t;
        }

        public bool Update(int tick)
        {
            for (int i = worldObjects.Count - 1; i >= 0; i--)
            {
                if (worldObjects[i] is IUpdatable)
                {
                    bool needsCommand = ((IUpdatable)worldObjects[i]).Update(tick);

                    if (needsCommand)
                    {
                        if (worldObjects[i] is Robot)
                        {
                            Robot r = (Robot)worldObjects[i];

                            // Gives Robot new tasks if possible and needed
                            if (r.currentNode == homeNode && r.currentRack == null && r.tasks.Count == 0)
                            {
                                if (storedRacks.Count >= 25) // Search for rack and place it at the home node
                                {
                                    int randomRackInt = 0;
                                    Rack randomRack = null;
                                    int randomRackNodeName = 0;

                                    bool checkRack = false;
                                    // Select a random rack
                                    while (!checkRack)
                                    {
                                        randomRackInt = random.Next(storedRacks.Count);
                                        randomRack = storedRacks[randomRackInt];
                                        randomRackNodeName = Convert.ToInt32(randomRack.currentNode.NodeName);

                                        if (randomRackNodeName != Convert.ToInt32(this.homeNode.NodeName))
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

                                    if (placeRackNode != null)
                                    {
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
                            }
                        }
                        else if (worldObjects[i] is Transport)
                        {
                            Transport t = (Transport)worldObjects[i];
                            t.UpdatePosition();

                            // Transport reached loader
                            if (t.reachedLoader)
                            {
                                // Clear all empty racks
                                foreach (Rack r in emptyRacks)
                                {
                                    if (r.currentNode == this.homeNode && r.delete != true)
                                    {
                                        r.delete = true;
                                        r.needsUpdate = true;
                                    }
                                }

                                if (!t.createdRacks)
                                {
                                    t.createdRacks = true;

                                    int generatedRacks = 0;
                                    // Generates random count of racks to be placed in storage
                                    if (storedRacks.Count <= 25)
                                    {
                                        generatedRacks = random.Next(3, 5);
                                    } else
                                    {
                                        generatedRacks = 0;
                                    }

                                    // Spaw new racks when truck has reached the loader
                                    for (int a = generatedRacks; a > 0; a--)
                                    {
                                        newRacks.Add(CreateRack(homeNode.x + 1, homeNode.y + 0.2, homeNode.z, homeNode));
                                    }
                                    Console.Write("Stored racks: " + storedRacks.Count);
                                    Console.WriteLine(" New racks: " + newRacks.Count);
                                }
                            }
                        }

                        // Send Model through socket
                        SendCommandToObservers(new UpdateModel3DCommand(worldObjects[i]));

                        // Remove object from worldobjects
                        if (worldObjects[i].delete == true)
                        {
                            worldObjects.RemoveAt(i);
                        }
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