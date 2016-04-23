using MechWars.MapElements.Orders.Products;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class ProductionOrder : Order<Building>
    {
        int paid;
        float progress;

        bool resourcesDepleted;

        public Building Building { get; private set; }
        public Product Product { get; private set; }

        public bool Done { get; private set; }

        public ProductionOrder(Building orderedBuilding, Product product)
            : base("Production", orderedBuilding)
        {
            Building = orderedBuilding;
            Product = product;
        }

        protected override bool RegularUpdate()
        {

            if (!Done)
            {
                if (resourcesDepleted)
                {
                    if (Building.army.resources > 0)
                    {
                        resourcesDepleted = false;
                        Debug.Log(string.Format("Production of {0} in {1} resumed.", Product, Building));
                    }
                    else return false;
                }

                float time = Product.ProductionTime;
                int cost = Product.Cost;
                var costLeft = cost - paid;

                float dTotal;
                if (time > 0)
                {
                    float dt = Time.deltaTime;
                    dTotal = dt / time;
                }
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
                        Done = true;
                    }
                    int resources = Building.army.resources;
                    if (intProgress >= resources)
                    {
                        intProgress = resources;
                        if (!resourcesDepleted)
                        {
                            resourcesDepleted = true;
                            Debug.Log(string.Format("Resources depleted! Production of {0} in {1} on hold.", Product, Building));
                        }
                    }

                    Building.army.resources -= intProgress;
                    paid += intProgress;
                }


                //float resources = Building.army.resources;
                //if (dCost > resources)
                //{
                //    dProgress *= resources / dCost;
                //    dCost = resources;

                //    if (!resourcesDepleted)
                //    {
                //        resourcesDepleted = true;
                //        Debug.Log(string.Format(
                //            "Resources depleted! Production of {0} in {1} on hold.", Product, Building));
                //    }
                //}
                //else
                //{
                //    if (resourcesDepleted)
                //    {
                //        resourcesDepleted = false;
                //        Debug.Log(string.Format("Production of {0} in {1} resumed.", Product, Building));
                //    }

                //    Building.army.resources -= dCost;
                //    __paid += dCost;
                //    __progress += dProgress;

                //    if (__progress == 1)
                //        Done = true;
                //}
            }

            return Done;
        }

        protected override bool StoppingUpdate()
        {
            if (!Done) Revert();
            return true;
        }

        public void Revert()
        {
            Building.army.resources += paid;
            paid = 0;
            progress = 0;
            Done = false;
        }

        public override string ToString()
        {
            return string.Format("Production [ {0} ]", Product);
        }
    }
}
