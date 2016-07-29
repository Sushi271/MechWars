using MechWars.MapElements.Orders.Products;

namespace MechWars.MapElements
{
    public class BuildingConstructionInfo
    {
        public BuildingProduct BuildingProduct { get; private set; }
        public int Paid { get; set; }
        public float TotalProgress { get; set; }

        public Building Building { get { return BuildingProduct.Building; } }
        public int Cost { get { return BuildingProduct.Cost; } }
        public float ConstructionTime { get { return BuildingProduct.ProductionTime; } }

        public BuildingConstructionInfo(BuildingProduct buildingProduct, int alreadyPaid)
        {
            BuildingProduct = buildingProduct;
            Paid = alreadyPaid;
            TotalProgress = (float)Paid / Cost;
        }

        public BuildingConstructionInfo Clone(Building newBuilding)
        {
            return new BuildingConstructionInfo(
                BuildingProduct.Clone(newBuilding), Paid);
        }
    }
}