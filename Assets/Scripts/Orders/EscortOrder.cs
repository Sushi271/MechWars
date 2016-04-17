using System.Collections.Generic;
using MechWars.MapElements;
using MechWars.Pathfinding;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.Orders
{
    public class EscortOrder : Order
    {
        public MapElement Target { get; private set; }

        public EscortOrder(Unit orderedUnit, MapElement target)
            : base("Escort", orderedUnit)
        {
            Target = target;
        }

        protected override bool RegularUpdate()
        {
            return false;
        }

        protected override bool StoppingUpdate()
        {
            return true;
        }

        public override string ToString()
        {
            return string.Format("Escort [ {0} ]", Target);
        }
    }
}
