using MechWars.MapElements.Statistics;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class IdleOrder : Order<MapElement>
    {
        AttackOrder attack;
        MapElement autoAttackTarget;
        bool targetLost;
        
        public IdleOrder(Unit orderedUnit)
            : base("Idle", orderedUnit)
        {
        }

        public IdleOrder(Building orderedBuilding)
            : base("Idle", orderedBuilding)
        {
        }

        protected override bool RegularUpdate()
        {
            if (MapElement.canAttack)
            {
                if (autoAttackTarget == null)
                {
                    autoAttackTarget = AcquireTarget();
                    if (autoAttackTarget != null)
                    {
                        if (MapElement is Unit)
                            attack = new AttackOrder((Unit)MapElement, autoAttackTarget);
                        else if (MapElement is Building)
                            attack = new AttackOrder((Building)MapElement, autoAttackTarget);
                        else throw new System.Exception("MapElement is neither Unit nor Building.");
                        
                        targetLost = false;
                    }
                }
                else
                {
                    if (targetLost || !autoAttackTarget.Alive
                        || !MapElement.MapElementInRange(autoAttackTarget))
                    {
                        targetLost = true;
                        if (!attack.Stopping) attack.Stop();
                        if (attack.Stopped)
                        {
                            attack = null;
                            autoAttackTarget = null;
                        }
                    }
                }
            }

            if (attack != null)
                attack.Update();
            else CasualIdleBehaviour();

            return false;
        }

        protected override bool StoppingUpdate()
        {
            if (autoAttackTarget != null)
            {
                if (!attack.AttackingInProgress) return true;

                attack.Update();
                if (!attack.Stopped) return false;
                else
                {
                    attack = null;
                    autoAttackTarget = null;
                    return true;
                }
            }
            else
            {
                // Finish CasualIdleBehaviour()
                return true;
            }
        }

        protected override void TerminateCore()
        {
        }

        protected override void OnStopCalled()
        {
            if (attack != null)
                attack.Stop();
        }

        MapElement AcquireTarget()
        {
            var range = MapElement.Stats[StatNames.Range];
            if (range == null) return null;

            var coords =
                from c in CoordsInRangeSquare(range.Value)
                where (c - MapElement.Coords).magnitude <= range.Value
                where Globals.FieldReservationMap.CoordsInside(c)
                let me = Globals.FieldReservationMap[c]
                where me != null && me.army != null && me.army != MapElement.army
                select me;
            if (coords.Count() == 0) return null;

            var target = coords.SelectMin(me => Vector2.Distance(me.Coords, MapElement.Coords));
            return target;
        }

        IEnumerable<IVector2> CoordsInRangeSquare(float range)
        {
            int xFrom = Mathf.CeilToInt(MapElement.X - range);
            int xTo = Mathf.FloorToInt(MapElement.X + range);
            int yFrom = Mathf.CeilToInt(MapElement.Y - range);
            int yTo = Mathf.FloorToInt(MapElement.Y + range);

            for (int y = yFrom; y <= yTo; y++)
                for (int x = xFrom; x <= xTo; x++)
                    yield return new IVector2(x, y);
        }

        void CasualIdleBehaviour()
        {
            // rotate around from time to time or sth
        }

        public override string ToString()
        {
            return string.Format("Idle");
        }
    }
}
