namespace MechWars.MapElements.Orders
{
    public abstract class OrderResult
    {
        public abstract bool Success { get; }
        public string Message { get; private set; }

        public OrderResult(string message = "")
        {
            Message = message;
        }
    }
}
