namespace MechWars.MapElements.Orders.Actions.Args
{
    public class ResourceOrderActionArgs : OrderActionArgs
    {
        public const string ResourceArgName = "resource";

        public ResourceOrderActionArgs(Resource resource)
            : base(new OrderActionArg(ResourceArgName, resource))
        {
        }
    }
}
