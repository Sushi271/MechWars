using MechWars.MapElements.Statistics;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class CollectOrder : Order<Unit>
    {
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

        public CollectOrder(Unit orderedUnit, Resource resource)
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

        bool MakeCollect()
        {
            if (!Unit.canCollectResources)
                throw new System.Exception(string.Format("Unit {0} cannot collect/deposit resources.", Unit));

            var collectRateStat = Unit.Stats[StatNames.CollectRate];
            var carriedResourceStat = Unit.Stats[StatNames.CarriedResource];
            if (collectRateStat == null || carriedResourceStat == null)
                throw new System.Exception(string.Format(
                    "Unit {0} claims it can collect resources, but it doesn't have Stats: {1} & {2}, that are necessary.",
                    Unit, StatNames.CollectRate, StatNames.CarriedResource));

            var collectRate = collectRateStat.Value;
            var carriedResource = carriedResourceStat.Value;
            var tankCapacity = carriedResourceStat.MaxValue;
            var tankRoomLeft = tankCapacity - carriedResource;
            
            var dCollect = collectRate * Time.deltaTime;

            bool finished = false;
            float limit = Mathf.Min(tankRoomLeft, Resource.value);
            if (dCollect >= limit)
            {
                dCollect = limit;
                finished = true;
            }

            Resource.value -= dCollect;
            carriedResourceStat.Value += dCollect;

            return finished;
        }
        
        public override string ToString()
        {
            return string.Format("Collect [ {0} ]", Resource);
        }
    }
}
