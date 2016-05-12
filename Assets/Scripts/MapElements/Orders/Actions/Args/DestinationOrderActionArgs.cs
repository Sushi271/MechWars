using MechWars.Utils;

namespace MechWars.MapElements.Orders.Actions.Args
{
    public class DestinationOrderActionArgs : OrderActionArgs
    {
        public const string DestinationArgName = "destination";

        public DestinationOrderActionArgs(IVector2 destination)
            : base(new OrderActionArg(DestinationArgName, destination))
        {
        }
    }
}
