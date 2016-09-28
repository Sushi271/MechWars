namespace MechWars.MapElements.Orders
{
    public class SuccessOrderResult : OrderResult
    {
        public override bool Success { get { return true; } }

        public SuccessOrderResult(string message = "")
            : base(message)
        {
        }
    }
}
