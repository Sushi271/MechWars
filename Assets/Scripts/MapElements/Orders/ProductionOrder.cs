using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class ProductionOrder : Order<Building>
    {
        float progress;
        float paid;

        bool resourcesDepleted;

        public Building Building { get; private set; }
        public Product Product { get; private set; }
        //public string ProductName { get { return ProductPrefab.GetComponent<MapElement>().name; } }

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
                float time = Product.ProductionTime;
                float cost = Product.Cost;

                if (time == 0)
                    throw new System.Exception("Production time must be > 0.");
                float dt = Time.deltaTime;
                float dProgress = dt / time;
                float dCost = dProgress * cost;

                if (progress + dProgress > 1)
                {
                    dProgress = 1 - progress;
                    dCost = cost - paid;
                }

                float resources = Building.army.resources;
                if (dCost > resources)
                {
                    dProgress *= resources / dCost;
                    dCost = resources;

                    if (!resourcesDepleted)
                    {
                        resourcesDepleted = true;
                        Debug.Log(string.Format(
                            "Resources depleted! Production of {0} in {1} on hold.", Product, Building));
                    }
                }
                else
                {
                    if (resourcesDepleted)
                    {
                        resourcesDepleted = false;
                        Debug.Log(string.Format("Production of {0} in {1} resumed.", Product, Building));
                    }

                    Building.army.resources -= dCost;
                    paid += dCost;
                    progress += dProgress;

                    if (progress == 1)
                        Done = true;
                }
            }

            return Done;
        }

        protected override bool StoppingUpdate()
        {
            if (!Done)
            {
                Building.army.resources += paid;
            }
            return true;
        }

        public override string ToString()
        {
            return string.Format("Production [ {0} ]", Product);
        }
    }
}
