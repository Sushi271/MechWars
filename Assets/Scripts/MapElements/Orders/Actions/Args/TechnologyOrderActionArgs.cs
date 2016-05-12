namespace MechWars.MapElements.Orders.Actions.Args
{
    public class TechnologyOrderActionArgs : OrderActionArgs
    {
        public const string TechnologyArgName = "technology";

        public TechnologyOrderActionArgs(Technology technology)
            : base(new OrderActionArg(TechnologyArgName, technology))
        {
        }
    }
}
