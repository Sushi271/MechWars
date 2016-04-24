using MechWars.MapElements.Statistics;
using UnityEngine;

namespace MechWars.MapElements.Orders.Products
{
    public class BuildingProduct : Product
    {
        public Building Building { get; private set; }

        public BuildingProduct(Building product)
        {
            Building = product;
            
            Name = Building.mapElementName;
            Cost = Building.ConstructionInfo.Cost;
            ProductionTime = Building.ConstructionInfo.ConstructionTime;
        }

        public override string ToString()
        {
            return string.Format("{0} [ {1} ]", base.ToString(), Building);
        }
    }
}