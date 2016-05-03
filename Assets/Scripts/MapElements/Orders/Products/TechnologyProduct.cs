namespace MechWars.MapElements.Orders.Products
{
    public class TechnologyProduct : Product
    {
        public Technology Technology { get; private set; }

        public TechnologyProduct(Building producer, Technology technology)
        {
            Technology = technology;

            var devOp = producer.technologyDevelopmentOptions.Find(@do => @do.technology == technology);
            if (devOp == null)
                throw new System.Exception(string.Format("Building {0} cannot develop Technology {1}", producer, Technology));

            Name = technology.technologyName;
            Cost = devOp.cost;
            ProductionTime = devOp.developmentTime;
        }

        public override string ToString()
        {
            return string.Format("{0} [ {1} ]", base.ToString(), Technology);
        }
    }
}