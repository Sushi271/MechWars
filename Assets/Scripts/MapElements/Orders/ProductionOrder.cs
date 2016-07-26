using MechWars.MapElements.Orders.Products;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class ProductionOrder : Order
    {
        public override string Name { get { return "Production"; } }
        protected override bool CanStop { get { return true; } }

        public int Paid { get; private set; }
        float progress;

        bool resourcesDepleted;

        public Building Building { get; private set; }
        public Product Product { get; private set; }
        
        public bool DontRevert { get; set; }

        public ProductionOrder(Building building, Product product, int paid = 0)
            : base(building)
        {
            Building = building;
            Product = product;
            Paid = paid;
        }

        protected override void OnUpdate()
        {
            if (State == OrderState.Stopping)
            {
                Revert();
                return;
            }

            var army = Building.Army;
            if (resourcesDepleted)
            {
                if (army.resources > 0)
                {
                    resourcesDepleted = false;
                    Debug.Log(string.Format("Production of {0} in {1} resumed.", Product, Building));
                }
                else return;
            }

            float time = Product.ProductionTime;
            int cost = Product.Cost;
            var costLeft = cost - Paid;

            float dTotal;
            if (time > 0)
                dTotal = Time.deltaTime / time;
            else dTotal = 1;
            if (dTotal > 1) dTotal = 1;

            float dProgress = dTotal * cost;
            progress += dProgress;
            int intProgress = (int)progress;

            if (intProgress > 0 || cost == 0)
            {
                progress -= intProgress;
                if (intProgress >= costLeft)
                {
                    intProgress = costLeft;
                    Succeed();
                }
                int resources = army.resources;
                if (intProgress >= resources)
                {
                    intProgress = resources;
                    if (!resourcesDepleted)
                    {
                        resourcesDepleted = true;
                        Debug.Log(string.Format("Resources depleted! Production of {0} in {1} on hold.", Product, Building));
                    }
                }

                army.resources -= intProgress;
                Paid += intProgress;
            }
        }

        public void Revert()
        {
            if (DontRevert) return;

            Building.Army.resources += Paid;
            Paid = 0;
            progress = 0;
        }

        protected override string SpecificsToString()
        {
            return string.Format("{0}", Product);
        }
    }
}
