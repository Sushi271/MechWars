using MechWars.Utils;
using System.Collections.Generic;

namespace MechWars.MapElements.Orders.Actions
{
    public interface IOrderActionArgs
    {
        IVector2 Destination { get;  }
        IEnumerable<MapElement> Targets { get; }
        ICanCreateOrderArgs CanCreateOrderArgs { get; }
    }
}