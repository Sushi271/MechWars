using MechWars.Utils;
using System.Collections.Generic;

namespace MechWars.MapElements.Orders.Actions
{
    public class OrderActionArgs
    {
        public IVector2 Destination { get; private set; }
        public IEnumerable<MapElement> Targets { get; private set; }

        public OrderActionArgs(IVector2 destination, IEnumerable<MapElement> targets)
        {
            Destination = destination;
            Targets = targets;
        }
    }
}