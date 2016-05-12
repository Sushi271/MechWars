namespace MechWars.MapElements.Orders.Actions.Args
{
    public class UnitOrderActionArgs : OrderActionArgs
    {
        public const string UnitArgName = "unit";

        public UnitOrderActionArgs(Unit target)
            : base(new OrderActionArg(UnitArgName, target))
        {
        }
    }
}
