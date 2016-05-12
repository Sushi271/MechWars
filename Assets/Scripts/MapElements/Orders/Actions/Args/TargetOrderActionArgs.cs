namespace MechWars.MapElements.Orders.Actions.Args
{
    public class TargetOrderActionArgs : OrderActionArgs
    {
        public const string TargetArgName = "target";

        public TargetOrderActionArgs(MapElement target)
            : base(new OrderActionArg(TargetArgName, target))
        {
        }
    }
}
