using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;

namespace Models {
    public class World : IObservable<Command>, IUpdatable
    {
        private List<_3DModel> worldObjects = new List<_3DModel>();
        private List<IObserver<Command>> observers = new List<IObserver<Command>>();
        
        public World() {
            Robot r = CreateRobot(0,0,0);
            Rack ra = CreateRack(1, 1, 1);
            Transport tr = CreateTransport(2, 2, 2);
            r.Move(4.6, 0, 13);
        }

        private Robot CreateRobot(double x, double y, double z) {
            Robot ro = new Robot(x,y,z,0,0,0);
            worldObjects.Add(ro);
            return ro;
        }
        private Rack CreateRack(double x, double y, double z)
        {
            Rack ra = new Rack(x, y, z, 0, 0, 0);
            worldObjects.Add(ra);
            return ra;
        }
        private Transport CreateTransport(double x, double y, double z)
        {
            Transport tr = new Transport(x, y, z, 0, 0, 0);
            worldObjects.Add(tr);
            return tr;
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
            foreach(_3DModel m3d in worldObjects) {
                obs.OnNext(new UpdateModel3DCommand(m3d));
            }
        }

        public bool Update(int tick)
        {
            for(int i = 0; i < worldObjects.Count; i++) {
                _3DModel u = worldObjects[i];

                if(u is IUpdatable) {
                    bool needsCommand = ((IUpdatable)u).Update(tick);

                    if(needsCommand) {
                        u.Move(u.x, u.y + 0.01, u.z); // Change position of Model
                        SendCommandToObservers(new UpdateModel3DCommand(u));
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