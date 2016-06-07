using MechWars.MapElements.Statistics;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class DepositOrder : Order
    {
        public override string Name { get { return "Deposit"; } }
        protected override bool CanStop { get { return true; } }

        public Unit Unit { get; private set; }
        public Building Deposit { get; private set; }

        float depositRate;
        Stat carriedResourceStat;

        float progress;

        public DepositOrder(Unit unit, Building deposit)
            : base(unit)
        {
            Unit = unit;
            Deposit = deposit;
        }

        protected override void OnStart()
        {
            Stat depositRateStat = null;
            TryFail(OrderResultAsserts.AssertMapElementHasStat(MapElement, StatNames.CarriedResource, out carriedResourceStat));
            TryFail(OrderResultAsserts.AssertMapElementHasStat(MapElement, StatNames.DepositRate, out depositRateStat));
            TryFail(OrderResultAsserts.AssertUnitIsNeighbourOf(Unit, Deposit));
            TryFail(OrderResultAsserts.AssertBuildingIsNotUnderConstruction(Deposit));
            TryFail(OrderResultAsserts.AssertMapElementIsNotDying(Deposit));
            TryFail(OrderResultAsserts.AssertBuildingIsResourceDeposit(Deposit));
            if (Failed) return;

            if (carriedResourceStat.Value == 0)
            {
                Succeed();
                return;
            }

            depositRate = depositRateStat.Value;
        }

        protected override void OnUpdate()
        {
            if (State == OrderState.Stopping) return;
            if (Deposit.Dying)
            {
                Succeed();
                return;
            }

            var carriedResource = (int)carriedResourceStat.Value;

            var dProgress = depositRate * Time.deltaTime;
            progress += dProgress;
            var intProgress = (int)progress;
            
            if (intProgress > 0)
            {
                if (intProgress >= carriedResource)
                {
                    intProgress = carriedResource;
                    Succeed();
                }

                progress -= intProgress;
                Deposit.army.resources += intProgress;
                carriedResourceStat.Value -= intProgress;
                Unit.additionalResourceValue -= intProgress;
            }
        }

        protected override string SpecificsToString()
        {
            return Deposit.ToString();
        }
    }
}
