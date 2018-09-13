using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;

namespace Models {
    public class World : IObservable<Command>, IUpdatable
    {
        private List<Model3D> worldObjects = new List<Model3D>();
        private List<IObserver<Command>> observers = new List<IObserver<Command>>();
        
        public World() {
            Robot robot = CreateRobot(10, 0.15, 10);
            //robot.Move(0, 0, 0);

            Rack rack = CreateRack(1, 3, 1);
            //Transport transport = CreateTransport(2, 2, 2);
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

        public IDisposable Subscribe(IObserver<Command> observer)
        {
            if (!observers.Contains(observer)) {
                observers.Add(observer);

                SendCreationCommandsToObserver(observer);
            }
            return new Unsubscriber<Command>(observers, observer);
        }

        private void SendCommandToObservers(Command c) {
            for(int i = 0; i < this.observers.Count; i++) {
                this.observers[i].OnNext(c);
            }
        }

        private void SendCreationCommandsToObserver(IObserver<Command> obs) {
            foreach(Model3D m3d in worldObjects) {
                obs.OnNext(new UpdateModel3DCommand(m3d));
            }
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
                        double y;
                        if (m3d.y >= 10) { y = 0.15; }
                        else { y = m3d.y + 0.10; }
                        m3d.Move(m3d.x, y, m3d.z);                              // Change position of Model
                        SendCommandToObservers(new UpdateModel3DCommand(m3d));  // Send Model through socket
                    }
                }
            }

            return true;
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