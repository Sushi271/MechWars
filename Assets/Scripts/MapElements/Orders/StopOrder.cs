namespace MechWars.MapElements.Orders
{
    public class StopOrder : Order
    {
        public override string Name { get { return "Stop"; } }
        
        public StopOrder(MapElement mapElement)
            : base(mapElement)
        {
        }

        protected override void OnStart()
        {
            Succeed();           
        }
    }
}
