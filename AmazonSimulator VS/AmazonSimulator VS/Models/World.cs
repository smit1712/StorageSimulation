using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using System.Threading;
namespace Models {
    public class World : IObservable<Command>, IUpdatable
    {
        private List<Model3D> worldObjects = new List<Model3D>();
        private List<IObserver<Command>> observers = new List<IObserver<Command>>();

        public World() {
            Thread RobotThread = new Thread(() => CreateRobot(10, 0.15, 10));
            RobotThread.Start();
            
            Thread RackThread = new Thread(() => CreateRack(0, 0.15, 0));
            RackThread.Start();

            Thread TransportThread = new Thread(() => CreateTransport(-1.0, 0.4, -10));
            TransportThread.Start();
        }

        private Robot CreateRobot(double x, double y, double z) {
            Robot r = new Robot(x, y, z, 0, 0, 0);
            worldObjects.Add(r);
            return r;
        }

        private Rack CreateRack(double x, double y, double z)
        {
            Rack r = new Rack(x, y, z, 0, 0, 0);
            worldObjects.Add(r);
            return r;
        }

        private Transport CreateTransport(double x, double y, double z)
        {
            Transport t = new Transport(x, y, z, 0, 0, 0);
            worldObjects.Add(t);
            return t;
        }

        private Person CreatePerson(double x, double y, double z)
        {
            Person p = new Person(x, y, z, 0, 0, 0);
            worldObjects.Add(p);
            return p;
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
                                break;

                            case "robot":
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