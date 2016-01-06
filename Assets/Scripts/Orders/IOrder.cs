using System.Collections.Generic;
using MechWars.MapElements;

namespace MechWars.Orders
{
    public interface IOrder
    {
        string Name { get; }
        List<Unit> OrderedUnits { get; }
        bool Stopping { get; }
        bool Stopped { get; }
    }
}