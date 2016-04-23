namespace MechWars.MapElements.Orders
{
    public class Product
    {
        public string Name { get; private set; }
        public float Cost { get; private set; }
        public float ProductionTime { get; private set; }

        public Product(string name, float cost, float productionTime)
        {
            Name = name;
            Cost = cost;
            ProductionTime = productionTime;
        }

        public override string ToString()
        {
            return string.Format("Product \"{0}\"", Name);
        }
    }
}
