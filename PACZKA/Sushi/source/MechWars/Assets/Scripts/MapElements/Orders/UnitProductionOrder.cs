using MechWars.MapElements.Orders.Products;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class UnitProductionOrder : ComplexOrder
    {
        public override string Name { get { return "UnitProduction"; } }

        UnitProduct unitProduct;
        ProductionOrder productionOrder;

        bool spawning;
        bool noRoom;

        public Building ConstructingBuilding { get; private set; }
        public Unit ProducedUnit { get; private set; }

        public event System.Action<UnitProductionOrder, Unit> UnitSpawned;

        public UnitProductionOrder(Building orderedBuilding, UnitProduct unitProduct)
            : base(orderedBuilding)
        {
            this.unitProduct = unitProduct;

            ConstructingBuilding = orderedBuilding;
            ProducedUnit = unitProduct.Unit;
        }

        protected override void OnStart()
        {
            TryFail(OrderResultAsserts.AssertIsNotNull(ProducedUnit, "Produced Unit"));
            if (Failed) return;

            productionOrder = new ProductionOrder(ConstructingBuilding, unitProduct);
            GiveSubOrder(productionOrder);
        }

        protected override void OnUpdate()
        {
            if (spawning)
                if (TrySpawn())
                {
                    Succeed();
                    spawning = false;
                }
        }

        protected override void OnSubOrderUpdated()
        {
            ConstructingBuilding.additionalResourceValue = productionOrder.Paid;
        }

        protected override void OnSubOrderFinished()
        {
            if (TrySpawn())
            {
                Succeed();
                spawning = false;
            }
        }

        protected override void OnSubOrderStopped()
        {
            Succeed();
        }
        
        bool TrySpawn()
        {
            spawning = true;
            Unit newUnit = ConstructingBuilding.Spawn(ProducedUnit);
            if (newUnit == null)
            {
                if (!noRoom)
                {
                    noRoom = true;
                    Debug.Log(string.Format(
                        "Unit {0} production complete, but no room for it near Building {1}",
                        ProducedUnit, ConstructingBuilding));
                }
                return false;
            }

            newUnit.resourceValue = productionOrder.Product.Cost;
            ConstructingBuilding.additionalResourceValue = 0;
            Debug.Log(string.Format("Production of {0} complete.", ProducedUnit));
            
            if (UnitSpawned != null)
                UnitSpawned(this, newUnit);

            return true;
        }

        protected override string SpecificsToStringCore()
        {
            return string.Format("{0}", ProducedUnit);
        }
    }
}
