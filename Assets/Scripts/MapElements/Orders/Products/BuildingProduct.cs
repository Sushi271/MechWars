using MechWars.MapElements.Statistics;
using UnityEngine;

namespace MechWars.MapElements.Orders.Products
{
    public class BuildingProduct : Product
    {
        public Building Building { get; private set; }

        public BuildingProduct(Building building, int cost, float productionTime)
            : base(building.name, cost, productionTime)
        {
            Building = building;
        }

        public override string ToString()
        {
            return string.Format("{0} [ {1} ]", base.ToString(), Building);
        }
    }
}