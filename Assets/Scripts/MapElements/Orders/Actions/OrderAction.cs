using MechWars.MapElements.Orders.Actions.Args;
using UnityEngine;

namespace MechWars.MapElements.Orders.Actions
{
    public abstract class OrderAction<T> : MonoBehaviour
        where T : MapElement
    {
        public abstract IOrder CreateOrder(T orderExecutor, OrderActionArgs args);

        protected void AssertOrderActionArgsTypeValid<T>(OrderActionArgs args)
            where T : OrderActionArgs
        {
            if (args == null)
                throw new System.ArgumentException("\"OrderActionArgs\" args argument must not be NULL.");

            if (!(args is T))
                throw new System.ArgumentException(string.Format(
                    "Method \"CreateOrder\" in class {0} must take {1} as \"OrderActionArgs args\" argument.",
                    GetType().Name, typeof(T).Name));
        }
    }
}
