using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using System.Threading;
using AmazonSimulator;

namespace Models {
    public class World : IObservable<Command>, IUpdatable
    {
        // All models
        private List<Model3D> worldObjects = new List<Model3D>();
        // Robots
        private List<Robot> robots = new List<Robot>();
        // Racks
        private List<Rack> newRacks = new List<Rack>();
        private List<Rack> storedRacks = new List<Rack>();
        private List<Rack> emptyRacks = new List<Rack>();
        // Clients
        private List<IObserver<Command>> observers = new List<IObserver<Command>>();

        public World() {
            CreateTransport(-1.0, 0.4, -10);

            CreateRobot(10, 0.15, 10);
            CreateRobot(1, 0.15, 5);

            CreateRack(5, 0.15, 5);
            CreateRack(10, 0.15, 10);
            CreateRack(15, 0.15, 15);
            CreateRack(20, 0.15, 20);

            NodeCreator nodeCreator = new NodeCreator(30, 30);
            List<Node> NodeList = nodeCreator.GetNodeList();
            List<Node> CornerList = new List<Node>();
            foreach(Node n in NodeList)
            {
                if(n.GetAdjacentNode3() != null)
                {
                    CornerList.Add(n);
                }
            }
            List<Node> adjlist = new List<Node>();
            foreach (Node n in NodeList)
            {
                if (n.GetAdjacentNode3() != null)
                {
                    adjlist.Add(n.GetAdjacentNode3());
                    n.type = "adj";
                }
            }

            Dijkstra dijkstra = new Dijkstra(NodeList);
            List<Node> route = new List<Node>();
            route = dijkstra.GetBestRoute(NodeList[0],NodeList[12]);
            //worldObjects.AddRange(NodeList);
          //  worldObjects.AddRange(CornerList);
           // worldObjects.AddRange(adjlist);
           worldObjects.AddRange(route);
        }

       
        private Robot CreateRobot(double x, double y, double z) {
            Robot r = new Robot(x, y, z, 0, 0, 0);
            robots.Add(r);
            worldObjects.Add(r);
            return r;
        }

        private Rack CreateRack(double x, double y, double z)
        {
            Rack r = new Rack(x, y, z, 0, 0, 0);
            storedRacks.Add(r);
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
            foreach (Model3D m3d in worldObjects)
            {
                if (m3d is IUpdatable)
                {
                    bool needsCommand = ((IUpdatable)m3d).Update(tick);

                    if (needsCommand)
                    {
                        switch (m3d.type)
                        {
                            case "transport":
                                MoveTransport(m3d);
                                break;

                            case "robot":
                                Robot r = robots.Find(x => x.guid == m3d.guid);
                                MoveRobot(r);
                                break;
                            case "rack":
                                break;
                            case "node":                                
                                break;

                        }
                        SendCommandToObservers(new UpdateModel3DCommand(m3d));  // Send Model through socket
                    }
                }
            }

            return true;
        }

        double temporaryZ = 0;
        bool countedTick = false;
        int countTick = 0;
        public void MoveTransport(Model3D transport)
        {
            if (countTick > 0)
            {
                countTick++;
                Console.WriteLine(countTick);
                if (countTick > 30)
                {
                    countTick = 0;
                }
                transport.Move(transport.x, transport.y, transport.z);
                return;
            }

            if (transport.z >= 10 && !countedTick)
            {
                countTick = 1;
                countedTick = true;
                transport.Move(transport.x, transport.y, transport.z);
            }
            else if (transport.z > 30)
            {
                temporaryZ = 0;
                transport.Move(transport.x, transport.y, temporaryZ);
                countedTick = false;
                countTick = 0;
            }
            else
            {
                temporaryZ = transport.z + 0.15;
                transport.Move(transport.x, transport.y, temporaryZ);
            }
        }

        public void MoveRobot(Robot robot)
        {
            robot.SearchRack();

            if (robot.reachedDestiny)
            {
                if (!robot.hasRack)
                {
                    if (storedRacks.Count > 0)
                    {
                        Random rnd = new Random();
                        int index = rnd.Next(storedRacks.Count);
                        Rack rack = storedRacks[index];
                        robot.PickupRack(rack);
                        storedRacks.RemoveAt(index);
                    }
                } else
                {
                    emptyRacks.Add(robot.currentRack);
                    robot.DropRack();
                }
            }
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