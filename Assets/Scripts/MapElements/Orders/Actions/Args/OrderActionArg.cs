namespace MechWars.MapElements.Orders.Actions.Args
{
    public class OrderActionArg
    {
        public string Name { get; private set; }
        public object Value { get; private set; }
        
        public OrderActionArg(string name, object value = null)
        {
            Name = name;
            Value = value;
        }
    }
}
