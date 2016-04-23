using MechWars.MapElements.Statistics;
using System.Linq;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class DepositOrder : Order
    {
        public Building Refinery { get; set; }

        float depositProgress;

        public bool InRange
        {
            get
            {
                var allRefineryCoords = Refinery.AllCoords;
                return allRefineryCoords.Any(c =>
                    {
                        var dr = c - Unit.Coords;
                        return Mathf.Abs(dr.x) <= 1 && Mathf.Abs(dr.y) <= 1;
                    });
            }
        }

        public DepositOrder(Unit orderedUnit, Building refinery)
            : base("Deposit", orderedUnit)
        {
            Refinery = refinery;
        }

        protected override bool RegularUpdate()
        {
            if (InRange) return MakeDeposit();
            throw new System.Exception(string.Format("Order {0} called, when not in range.", Name));
        }

        protected override bool StoppingUpdate()
        {
            return true;
        }

        bool MakeDeposit()
        {
            if (!Unit.canCollectResources)
                throw new System.Exception(string.Format("Unit {0} cannot collect/deposit resources.", Unit));

            var depositRateStat = Unit.Stats[StatNames.DepositRate];
            var carriedResourceStat = Unit.Stats[StatNames.CarriedResource];
            if (depositRateStat == null || carriedResourceStat == null)
                throw new System.Exception(string.Format(
                    "Unit {0} claims it can collect resources, but it doesn't have Stats: {1} & {2}, that are necessary.",
                    Unit, StatNames.DepositRate, StatNames.CarriedResource));

            var depositRate = depositRateStat.Value;
            var carriedResource = (int)carriedResourceStat.Value;

            var dProgress = depositRate * Time.deltaTime;
            depositProgress += dProgress;
            int wholeRPDeposited = (int)depositProgress;
            
            bool finished = false;
            if (wholeRPDeposited > 0)
            {
                depositProgress -= wholeRPDeposited;
                if (wholeRPDeposited >= carriedResource)
                {
                    wholeRPDeposited = carriedResource;
                    finished = true;
                }

                Unit.army.resources += wholeRPDeposited;
                carriedResourceStat.Value -= wholeRPDeposited;
            }
            return finished;
        }

        public override string ToString()
        {
            return string.Format("Deposit [ {0} ]", Refinery);
        }
    }
}
