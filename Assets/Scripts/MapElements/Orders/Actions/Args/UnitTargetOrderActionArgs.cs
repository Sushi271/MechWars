namespace MechWars.MapElements.Orders.Actions.Args
{
    public class UnitTargetOrderActionArgs : OrderActionArgs
    {
        public const string TargetArgName = "target";

        public UnitTargetOrderActionArgs(Unit target)
            : base(new OrderActionArg(TargetArgName, target))
        {
        }
    }
}
