using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.MapElements.Orders_OLD
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
    }
}
