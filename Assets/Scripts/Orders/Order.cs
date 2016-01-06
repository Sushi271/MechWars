using System.Collections.Generic;
using MechWars.MapElements;

namespace MechWars.Orders
{
    public abstract class Order : IOrder
    {
        public string Name { get; private set; }
        public List<Unit> OrderedUnits { get; private set; }
        public bool Stopping { get; private set; }
        public bool Stopped { get; private set; }

        protected Order(string name, List<Unit> orderedUnits)
        {
            Name = name;
            OrderedUnits = orderedUnits;
        }

        public void Update(Unit unit)
        {
            if (Stopped) return;
            if (Stopping)
            {
                Stopped = StoppingUpdate(unit);
                if (Stopped) Stopping = false;
            }
            else Stopped = RegularUpdate(unit);
        }

        protected abstract bool RegularUpdate(Unit unit);
        protected abstract bool StoppingUpdate(Unit unit);

        public void Stop()
        {
            Stopping = true;
        }
    }
}
