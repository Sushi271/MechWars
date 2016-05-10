using MechWars.MapElements.Orders.Products;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class UnitProductionOrder : Order<Building>
    {
        ProductionOrder productionOrder;

        bool noRoom;
        bool spawning;

        public Building Building { get; private set; }
        public Unit Unit { get; private set; }


        public UnitProductionOrder(Building orderedBuilding, Unit producedUnit)
            : base("UnitProduction", orderedBuilding)
        {
            Building = orderedBuilding;
            Unit = producedUnit;

            productionOrder = new ProductionOrder(Building, new UnitProduct(Building, Unit));
        }

        protected override bool RegularUpdate()
        {
            productionOrder.Update();
            Building.additionalResourceValue = productionOrder.Paid;
            if (productionOrder.Done)
                return Spawn();
            return false;
        }

        protected override bool StoppingUpdate()
        {
            productionOrder.Update();
            Building.additionalResourceValue = productionOrder.Paid;
            if (productionOrder.Stopped)
            {
                if (productionOrder.Done && spawning)
                    return Spawn();
                else
                {
                    productionOrder.Revert();
                    return true;
                }
            }
            else return false;
        }

        protected override void TerminateCore()
        {
            productionOrder.Revert();
            Building.additionalResourceValue = 0;
        }

        bool Spawn()
        {
            spawning = true;
            Unit newUnit = Building.Spawn(Unit);
            if (newUnit == null)
            {
                if (!noRoom)
                {
                    noRoom = true;
                    Debug.Log(string.Format(
                        "Unit {0} production complete, but no room for it near Building {1}", Unit, Building));
                }
                return false;
            }

            newUnit.resourceValue = productionOrder.Product.Cost;
            Building.additionalResourceValue = 0;
            return true;
        }

        protected override void OnStopCalled()
        {
            productionOrder.Stop();
        }

        protected override string SpecificsToString()
        {
            return string.Format("{0}", Unit);
        }
    }
}
