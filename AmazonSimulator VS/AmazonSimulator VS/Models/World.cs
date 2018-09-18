using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using System.Threading;
namespace Models {
    public class World : IObservable<Command>, IUpdatable
    {
        private List<Model3D> worldObjects = new List<Model3D>();
        private List<Robot> robots = new List<Robot>();
        private List<Rack> newRacks = new List<Rack>();
        private List<Rack> storedRacks = new List<Rack>();
        private List<Rack> emptyRacks = new List<Rack>();

        private List<IObserver<Command>> observers = new List<IObserver<Command>>();

        public World() {
            CreateRobot(1, 0.15, 5);
            CreateRack(10, 0.15, 5);
            CreateRack(10, 0.15, 10);
            CreateRack(10, 0.15, 15);
            CreateRack(10, 0.15, 20);
            //CreateRack(1, 0.15, 5);

            //CreateTransport(-1.0, 0.4, -10);
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
                        }
                        SendCommandToObservers(new UpdateModel3DCommand(m3d));  // Send Model through socket
                    }
                }
            }

            return true;
        }

        double temporaryZ = 0;
        public void MoveTransport(Model3D transport)
        {
            if (transport.z > 10.5 && transport.z < 10.70)
            {
                //Thread.Sleep(5000);
                transport.Move(transport.x, transport.y, transport.z);
            }
            if (transport.z > 30)
            {
                temporaryZ = 0;
                transport.Move(transport.x, transport.x, temporaryZ);
            }
            else
            {
                temporaryZ = temporaryZ + 0.15;
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