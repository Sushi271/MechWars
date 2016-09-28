namespace MechWars.MapElements.Orders
{
    public class FailOrderResult : OrderResult
    {
        public override bool Success { get { return false; } }

        public FailOrderResult(string message = "")
            : base(message)
        {
        }
    }
}
