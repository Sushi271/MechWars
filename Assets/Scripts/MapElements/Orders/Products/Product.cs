namespace MechWars.MapElements.Orders.Products
{
    public class Product
    {
        public string Name { get; protected set; }
        public float Cost { get; protected set; }
        public float ProductionTime { get; protected set; }

        protected Product()
        {
        }

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
