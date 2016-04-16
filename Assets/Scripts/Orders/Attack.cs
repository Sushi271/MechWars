using System.Collections.Generic;
using MechWars.MapElements;
using MechWars.Pathfinding;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.Orders
{
    public class Attack : Order
    {
        public MapElement Target { get; private set; }

        public Attack(Unit orderedUnit, MapElement target)
            : base("Attack", orderedUnit)
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
            return string.Format("Attack [ {0} ]", Target);
        }
    }
}
