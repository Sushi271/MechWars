using MechWars.MapElements.Statistics;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class CollectOrder : Order
    {
        public override string Name { get { return "Collect"; } }
        protected override bool CanStop { get { return true; } }

        public Unit Unit { get; private set; }
        public Resource Resource { get; private set; }

        float collectRate;
        Stat carriedResourceStat;

        float progress;

        public CollectOrder(Unit unit, Resource resource)
            : base(unit)
        {
            Unit = unit;
            Resource = resource;
        }

        protected override void OnStart()
        {
            Stat collectRateStat = null;
            TryFail(OrderResultAsserts.AssertMapElementHasStat(MapElement, StatNames.CarriedResource, out carriedResourceStat));
            TryFail(OrderResultAsserts.AssertMapElementHasStat(MapElement, StatNames.CollectRate, out collectRateStat));
            TryFail(OrderResultAsserts.AssertUnitIsNeighbourOf(Unit, Resource));
            if (Failed) return;

            if (Resource.value == 0 || carriedResourceStat.HasMaxValue)
            {
                Succeed();
                return;
            }

            collectRate = collectRateStat.Value;
        }

        protected override void OnUpdate()
        {
            if (State == OrderState.Stopping) return;
            if (Resource.value == 0)
            {
                Succeed();
                return;
            }

            var carriedResource = (int)carriedResourceStat.Value;
            var tankCapacity = (int)carriedResourceStat.MaxValue;
            var tankRoomLeft = tankCapacity - carriedResource;

            var dProgress = collectRate * Time.deltaTime;
            progress += dProgress;
            var intProgress = (int)progress;
            
            if (intProgress > 0)
            {
                int limit = Mathf.Min(tankRoomLeft, Resource.value);
                if (intProgress >= limit)
                {
                    intProgress = limit;
                    Succeed();
                }

                progress -= intProgress;
                Resource.value -= intProgress;
                carriedResourceStat.Value += intProgress;
                Unit.additionalResourceValue += intProgress;
            }
        }

        protected override string SpecificsToString()
        {
            return Resource.ToString();
        }
    }
}
