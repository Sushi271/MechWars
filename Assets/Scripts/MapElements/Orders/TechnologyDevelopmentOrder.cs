using MechWars.MapElements.Orders.Products;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class TechnologyDevelopmentOrder : Order<Building>
    {
        ProductionOrder productionOrder;

        public Building Building { get; private set; }
        public Technology Technology { get; private set; }
       
        public TechnologyDevelopmentOrder(Building orderedBuilding, Technology developedTechnology)
            : base("Development", orderedBuilding)
        {
            Building = orderedBuilding;
            Technology = developedTechnology;
            productionOrder = new ProductionOrder(Building, new TechnologyProduct(Building, Technology));
            
            Building.army.Technologies.StartDeveloping(Technology);
        }

        protected override bool RegularUpdate()
        {
            productionOrder.Update();
            Building.additionalResourceValue = productionOrder.Paid;
            if (productionOrder.Done)
            {
                Develop();
                return true;
            }
            return false;
        }

        protected override bool StoppingUpdate()
        {
            productionOrder.Update();
            Building.additionalResourceValue = productionOrder.Paid;
            if (productionOrder.Stopped)
            {
                if (productionOrder.Done)
                    Develop();
                else Cancel();
                return true;
            }
            else return false;
        }

        protected override void TerminateCore()
        {
            Cancel();
            Building.additionalResourceValue = 0;
        }

        void Develop()
        {
            var technologyGO = Object.Instantiate(Technology.gameObject);
            Building.army.Technologies.FinishDeveloping(technologyGO.GetComponent<Technology>());
            Building.additionalResourceValue = 0;
        }

        void Cancel()
        {
            productionOrder.Revert();
            Building.army.Technologies.CancelDeveloping(Technology);
        }

        protected override void OnStopCalled()
        {
            productionOrder.Stop();
        }

        public override string ToString()
        {
            return string.Format("Development [ {0} ]", Technology);
        }
    }
}
