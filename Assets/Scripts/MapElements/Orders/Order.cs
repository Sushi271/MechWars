﻿namespace MechWars.MapElements.Orders
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

        public void Stop()
        {
            Stopping = true;
            OnStopCalled();
        }

        protected virtual void OnStopCalled()
        {
        }
    }
}