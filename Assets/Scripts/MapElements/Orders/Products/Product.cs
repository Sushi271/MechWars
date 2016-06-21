namespace MechWars.MapElements.Orders.Products
{
    public abstract class Product
    {
        public string Name { get; private set; }
        public int Cost { get; private set; }
        public float ProductionTime { get; private set; }
        
        protected Product(string name, int cost, float productionTime)
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
