using MechWars.Utils;
using System.Collections.Generic;

namespace MechWars.MapElements.Orders.Actions
{
    public interface IOrderActionArgs
    {
        Player Player { get; }
        IVector2 Destination { get;  }
        IEnumerable<MapElement> Targets { get; }
        IBuildingPlacement BuildingPlacement { get; }
    }
}