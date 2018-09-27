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
        // Racks
        private List<Rack> newRacks = new List<Rack>();
        private List<Rack> storedRacks = new List<Rack>();
        private List<Rack> emptyRacks = new List<Rack>();
        // Clients
        private List<IObserver<Command>> observers = new List<IObserver<Command>>();

        public World() {
            CreateTransport(-1.0, 0.4, -10);

            CreateRobot(10, 0.15, 10);
            

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
                }
            }

            Dijkstra dijkstra = new Dijkstra(NodeList);
            List<Node> route = new List<Node>();
            //route = dijkstra.GetBestRoute(NodeList[0], NodeList[1]);            
            //worldObjects.AddRange(CornerList);
            //worldObjects.AddRange(adjlist);
            //worldObjects.AddRange(route);

            // Let Robot loop
            foreach(Node N in NodeList)
            {
                Robot testRobot = CreateRobot(1, 0.15, 5);
                route = dijkstra.GetBestRoute(NodeList[0], NodeList[Convert.ToInt32(N.NodeName)]);
                testRobot.AddTask(new RobotMove(route.ToArray()));
            }
           
        }

       
        private Robot CreateRobot(double x, double y, double z) {
            Robot r = new Robot(x, y, z, 0, 0, 0);
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
                        if  (m3d is Robot)
                        {
                            Robot r = (Robot)m3d;
                            // route defines an List of nodes
                            //r.AddTask(new RobotMove(route.ToArray()));
                        } else if (m3d is Transport)
                        {
                            Transport t = (Transport)m3d;
                            t.UpdatePosition();
                        } else if (m3d is Rack)
                        {

                        } else if (m3d is Node)
                        {

                        }

                        SendCommandToObservers(new UpdateModel3DCommand(m3d));
                        // Send Model through socket
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