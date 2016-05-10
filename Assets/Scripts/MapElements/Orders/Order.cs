namespace MechWars.MapElements.Orders
{
    public abstract class Order<T> : IOrder
        where T : MapElement
    {
        public string Name { get; private set; }
        public MapElement MapElement { get; private set; }
        public bool Stopping { get; private set; }
        public bool Stopped { get; private set; }

        protected Order(string name, T mapElement)
        {
            Name = name;
            MapElement = mapElement;
        }

        public void Update()
        {
            if (Stopped) return;
            if (Stopping)
            {
                Stopped = StoppingUpdate();
                if (Stopped) Stopping = false;
            }
            else Stopped = RegularUpdate();
        }

        protected abstract bool RegularUpdate();
        protected abstract bool StoppingUpdate();
        protected abstract void TerminateCore();

        public void Stop()
        {
            Stopping = true;
            OnStopCalled();
        }

        public void Terminate()
        {
            TerminateCore();
            Stopped = true;
        }

        protected virtual void OnStopCalled()
        {
        }

        protected virtual string SpecificsToString()
        {
            return string.Empty;
        }

        public override string ToString()
        {
            var specifics = SpecificsToString();
            return string.Format("{0}{1}", Name, 
                specifics != string.Empty ?
                " [ {1} ]" : string.Empty);
        }
    }
}
