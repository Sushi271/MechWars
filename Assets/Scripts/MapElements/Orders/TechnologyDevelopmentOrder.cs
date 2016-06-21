using MechWars.MapElements.Orders.Products;
using MechWars.MapElements.Statistics;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class TechnologyDevelopmentOrder : ComplexOrder
    {
        public override string Name { get { return "TechnologyDevelopment"; } }

        TechnologyProduct technologyProduct;
        ProductionOrder productionOrder;
        
        public Building ConstructingBuilding { get; private set; }
        public Technology DevelopedTechnology { get; private set; }

        public TechnologyDevelopmentOrder(Building orderedBuilding, TechnologyProduct technologyProduct)
            : base(orderedBuilding)
        {
            this.technologyProduct = technologyProduct;

            ConstructingBuilding = orderedBuilding;
            DevelopedTechnology = technologyProduct.Technology;
        }

        protected override void OnStart()
        {
            TryFail(OrderResultAsserts.AssertIsNotNull(DevelopedTechnology, "Developed Technology"));
            if (Failed) return;

            ConstructingBuilding.army.Technologies.StartDeveloping(DevelopedTechnology);
            productionOrder = new ProductionOrder(ConstructingBuilding, technologyProduct);
            GiveSubOrder(productionOrder);
        }

        protected override void OnSubOrderUpdated()
        {
            ConstructingBuilding.additionalResourceValue = productionOrder.Paid;
        }

        protected override void OnSubOrderFinished()
        {
            Develop();
            Succeed();
        }

        protected override void OnSubOrderStopped()
        {
            Cancel();
            Succeed();
        }
        
        protected override void OnTerminating()
        {
            Cancel();
        }

        void Develop()
        {
            var technologyGO = Object.Instantiate(DevelopedTechnology.gameObject);
            ConstructingBuilding.army.Technologies.FinishDeveloping(technologyGO.GetComponent<Technology>());
            ConstructingBuilding.additionalResourceValue = 0;
        }

        void Cancel()
        {
            ConstructingBuilding.army.Technologies.CancelDeveloping(DevelopedTechnology);
        }

        protected override string SpecificsToStringCore()
        {
            return string.Format("{0}", DevelopedTechnology);
        }
    }
}
