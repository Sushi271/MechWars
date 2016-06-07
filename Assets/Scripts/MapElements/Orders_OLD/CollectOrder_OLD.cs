using MechWars.MapElements.Statistics;
using UnityEngine;

namespace MechWars.MapElements.Orders_OLD
{
    public class CollectOrder_OLD : Order_OLD
    {
        float progress;

        public Unit Unit { get; private set; }
        public Resource Resource { get; set; }

        public bool InRange
        {
            get
            {
                var dr = Resource.Coords - Unit.Coords;
                return Mathf.Abs(dr.x) <= 1 && Mathf.Abs(dr.y) <= 1;
            }
        }

        public CollectOrder_OLD(Unit orderedUnit, Resource resource)
            : base("Collect", orderedUnit)
        {
            Unit = (Unit)MapElement;
            Resource = resource;
        }

        protected override bool RegularUpdate()
        {
            if (!Resource.Alive) return true;
            if (Resource.value == 0) return true;
            if (InRange) return MakeCollect();
            throw new System.Exception(string.Format("Order {0} called, when not in range.", Name));
        }

        protected override bool StoppingUpdate()
        {
            return true;
        }

        protected override void TerminateCore()
        {
        }

        bool MakeCollect()
        {
            var collectRateStat = Unit.Stats[StatNames.CollectRate];
            var carriedResourceStat = Unit.Stats[StatNames.CarriedResource];
            if (collectRateStat == null || carriedResourceStat == null)
                throw new System.Exception(string.Format(
                    "Unit {0} claims it can collect resources, but it doesn't have Stats: {1} & {2}, that are necessary.",
                    Unit, StatNames.CollectRate, StatNames.CarriedResource));

            var collectRate = collectRateStat.Value;
            var carriedResource = (int)carriedResourceStat.Value;
            var tankCapacity = (int)carriedResourceStat.MaxValue;
            var tankRoomLeft = tankCapacity - carriedResource;
            
            var dProgress = collectRate * Time.deltaTime;
            progress += dProgress;
            int intProgress = (int)progress;

            bool finished = false;
            if (intProgress > 0)
            {
                progress -= intProgress;
                int limit = Mathf.Min(tankRoomLeft, Resource.value);
                if (intProgress >= limit)
                {
                    intProgress = limit;
                    finished = true;
                }

                Resource.value -= intProgress;
                carriedResourceStat.Value += intProgress;
                Unit.additionalResourceValue += intProgress;
            }
            return finished;
        }

        protected override string SpecificsToString()
        {
            return string.Format("{0}", Resource);
        }
    }
}
