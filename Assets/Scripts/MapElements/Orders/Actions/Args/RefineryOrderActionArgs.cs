namespace MechWars.MapElements.Orders.Actions.Args
{
    public class RefineryOrderActionArgs : OrderActionArgs
    {
        public const string RefineryArgName = "refinery";

        public RefineryOrderActionArgs(Building refinery)
            : base(new OrderActionArg(RefineryArgName, refinery))
        {
            if (!refinery.isResourceDeposit)
                throw new System.Exception("Building given as \"refinery\" is not a resource deposit.");
        }
    }
}
