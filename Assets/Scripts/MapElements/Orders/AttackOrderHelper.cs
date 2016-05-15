using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public static class AttackOrderHelper
    {
        public static void AssertTargetsCanBeAttacked(IEnumerable<MapElement> targets)
        {
            var nonAttackable = targets.Where(t => !t.CanBeAttacked);
            if (!nonAttackable.Empty())
                throw new System.Exception(string.Format("Following MapElements cannot be attacked: {0}.",
                    nonAttackable.ToDebugMessage()));
        }

        public static MapElement PickTarget(MapElement mapElement, IEnumerable<MapElement> targets)
        {
            if (targets.Empty()) return null;
            return targets.SelectMin(t => Vector2.Distance(mapElement.Coords, t.GetClosestFieldTo(mapElement.Coords)));
        }
    }
}
