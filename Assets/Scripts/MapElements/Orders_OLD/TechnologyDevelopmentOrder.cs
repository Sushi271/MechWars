using MechWars.MapElements.Orders_OLD.Products;
using UnityEngine;

namespace MechWars.MapElements.Orders_OLD
{
    public class TechnologyDevelopmentOrder : Order_OLD
    {
        ProductionOrder productionOrder;

        public Building Building { get; private set; }
        public Technology Technology { get; private set; }
       
        public TechnologyDevelopmentOrder(Building orderedBuilding, TechnologyProduct technologyProduct)
            : base("Development", orderedBuilding)
        {
            Building = orderedBuilding;
            Technology = technologyProduct.Technology;
            productionOrder = new ProductionOrder(Building, technologyProduct);
                        
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

        protected override string SpecificsToString()
        {
            return string.Format("{0}", Technology);
        }
    }
}
