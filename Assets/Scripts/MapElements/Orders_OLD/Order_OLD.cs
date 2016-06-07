namespace MechWars.MapElements.Orders_OLD
{
    public abstract class Order_OLD
    {
        public string Name { get; private set; }
        public MapElement MapElement { get; private set; }
        public bool Stopping { get; private set; }
        public bool Stopped { get; private set; }

        protected Order_OLD(string name, MapElement mapElement)
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
                string.Format(" [ {0} ]", specifics) :
                string.Empty);
        }
    }
}
