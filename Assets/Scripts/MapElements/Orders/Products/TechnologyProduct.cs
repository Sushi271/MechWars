namespace MechWars.MapElements.Orders.Products
{
    public class TechnologyProduct : Product
    {
        public Technology Technology { get; private set; }

        public TechnologyProduct(Technology technology, int cost, float productionTime)
            : base(technology.technologyName, cost, productionTime)
        {
            Technology = technology;
        }

        public override string ToString()
        {
            return string.Format("{0} [ {1} ]", base.ToString(), Technology);
        }
    }
}