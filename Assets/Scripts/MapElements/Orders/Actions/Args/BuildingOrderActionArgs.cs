namespace MechWars.MapElements.Orders.Actions.Args
{
    public class BuildingOrderActionArgs : OrderActionArgs
    {
        public const string BuildingArgName = "building";

        public BuildingOrderActionArgs(Building building)
            : base(new OrderActionArg(BuildingArgName, building))
        {
        }
    }
}
