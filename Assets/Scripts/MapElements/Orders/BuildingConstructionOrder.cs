using MechWars.MapElements.Orders.Products;
using MechWars.MapElements.Statistics;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class BuildingConstructionOrder : ComplexOrder
    {
        public override string Name { get { return "BuildingContruction"; } }

        BuildingProduct buildingProduct;
        BuildingConstructionInfo constructionInfo;
        Stat buildingHP;

        ProductionOrder productionOrder;

        public Building ConstructingBuilding { get; private set; }
        public Building ConstructedBuilding { get; private set; }

        public BuildingConstructionOrder(Building orderedBuilding, BuildingProduct buildingProduct)
            : base(orderedBuilding)
        {
            this.buildingProduct = buildingProduct;

            ConstructingBuilding = orderedBuilding;
            ConstructedBuilding = buildingProduct.Building;

            constructionInfo = ConstructedBuilding.ConstructionInfo;
        }

        protected override void OnStart()
        {
            TryFail(OrderResultAsserts.AssertIsNotNull(ConstructedBuilding, "Constructed Building"));
            TryFail(OrderResultAsserts.AssertMapElementHasStat(ConstructedBuilding, StatNames.HitPoints, out buildingHP));
            TryFail(OrderResultAsserts.AssertMapElementIsNotDying(ConstructedBuilding));
            if (Failed) return;

            productionOrder = new ProductionOrder(ConstructingBuilding, buildingProduct, constructionInfo.Paid);
            productionOrder.DontRevert = true;
            GiveSubOrder(productionOrder);
        }

        protected override void OnUpdate()
        {
            if (ConstructedBuilding.Dying)
                productionOrder.Stop();
        }

        protected override void OnSubOrderUpdated()
        {
            if (SubOrder.State == OrderState.Stopped)
                return;
            UpdateConstructedBuilding();
        }

        protected override void OnSubOrderFinished()
        {
            FinalizeConstruction();
            Succeed();
        }

        protected override void OnSubOrderStopped()
        {
            ConstructedBuilding.army = null;
            Succeed();
        }

        void UpdateConstructedBuilding()
        {
            constructionInfo.Paid = productionOrder.Paid;
            var oldProgress = constructionInfo.TotalProgress;
            var newProgress = (float)constructionInfo.Paid / constructionInfo.Cost;
            var dProgress = newProgress - oldProgress;
            constructionInfo.TotalProgress = newProgress;

            ConstructedBuilding.resourceValue = constructionInfo.Paid;
            buildingHP.Value += dProgress * buildingHP.MaxValue;
        }

        void FinalizeConstruction()
        {
            ConstructedBuilding.FinishConstruction();
            Debug.Log(string.Format("Construction of {0} complete.", ConstructedBuilding));
        }

        protected override string SpecificsToStringCore()
        {
            return string.Format("{0}", ConstructedBuilding);
        }
    }
}
