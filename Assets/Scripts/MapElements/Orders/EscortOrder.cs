namespace MechWars.MapElements.Orders
{
    public class EscortOrder : Order
    {
        public MapElement Target { get; private set; }

        public EscortOrder(Unit orderedUnit, MapElement target)
            : base("Escort", orderedUnit)
        {
            Target = target;
        }

        protected override bool RegularUpdate()
        {
            return false;
        }

        protected override bool StoppingUpdate()
        {
            return true;
        }

        public override string ToString()
        {
            return string.Format("Escort [ {0} ]", Target);
        }
    }
}
