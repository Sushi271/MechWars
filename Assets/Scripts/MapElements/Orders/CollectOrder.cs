using MechWars.MapElements.Statistics;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class CollectOrder : Order<Unit>
    {
        public Unit Unit { get; private set; }

        public Resource Resource { get; set; }

        float collectProgress;

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
            var carriedResource = (int)carriedResourceStat.Value;
            var tankCapacity = (int)carriedResourceStat.MaxValue;
            var tankRoomLeft = tankCapacity - carriedResource;
            
            var dProgress = collectRate * Time.deltaTime;
            collectProgress += dProgress;
            int wholeRPCollected = (int)collectProgress;

            bool finished = false;
            if (wholeRPCollected > 0)
            {
                collectProgress -= wholeRPCollected;
                if (wholeRPCollected >= tankRoomLeft)
                {
                    wholeRPCollected = tankRoomLeft;
                    finished = true;
                }
                if (wholeRPCollected >= Resource.value)
                {
                    wholeRPCollected = Resource.value;
                    finished = true;
                }

                Resource.value -= wholeRPCollected;
                carriedResourceStat.Value += wholeRPCollected;
            }
            return finished;
        }
        
        public override string ToString()
        {
            return string.Format("Collect [ {0} ]", Resource);
        }
    }
}
