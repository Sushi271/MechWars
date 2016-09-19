using MechWars.MapElements;
using MechWars.MapElements.Orders.Actions;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;

namespace MechWars.AI
{
    public class AIOrderActionArgs : IOrderActionArgs
    {
        List<MapElement> targets;

        public Player Player { get; private set; }
        public IVector2 Destination { get; private set; }
        public IEnumerable<MapElement> Targets { get { return targets; } }
        public IBuildingPlacement BuildingPlacement { get; private set; }

        AIOrderActionArgs(Player aiPlayer, IVector2 destination, IEnumerable<MapElement> targets, AIBuildingPlacement placement)
        {
            Player = aiPlayer;
            Destination = destination;
            this.targets = new List<MapElement>(targets);
            BuildingPlacement = placement;
        }

        public AIOrderActionArgs(Player aiPlayer)
            : this(aiPlayer, default(IVector2), Enumerable.Empty<MapElement>(), null)
        {
        }

        public AIOrderActionArgs(Player aiPlayer, AIBuildingPlacement placement)
            : this(aiPlayer, default(IVector2), Enumerable.Empty<MapElement>(), placement)
        {
        }

        public AIOrderActionArgs(Player aiPlayer, MapElement target)
            : this(aiPlayer, default(IVector2), target.AsEnumerable(), null)
        {
        }
    }
}