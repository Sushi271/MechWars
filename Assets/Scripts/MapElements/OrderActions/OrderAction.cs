using MechWars.MapElements.Orders;
using UnityEngine;

namespace MechWars.MapElements.OrderActions
{
    public abstract class OrderAction : MonoBehaviour
    {
        public abstract IOrder CreateOrder(OrderActionArgs args);

    }
}
