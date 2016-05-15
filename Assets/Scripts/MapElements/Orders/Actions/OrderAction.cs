using MechWars.Human;
using MechWars.MapElements.Orders.Actions.Args;
using System.Collections.Generic;
using UnityEngine;

namespace MechWars.MapElements.Orders.Actions
{
    public abstract class OrderAction<T> : MonoBehaviour, IOrderAction
        where T : MapElement
    {
        public Color framesColor = Color.black;
        public Color FramesColor { get { return framesColor; } }

        public virtual bool AllowsMultiTarget { get { return false; } }
        public virtual bool AllowsHover { get { return false; } }
        public virtual bool IsAttack { get { return false; } }

        public virtual void FilterHoverCandidates(HumanPlayer player, HashSet<MapElement> candidates)
        {
            if (!AllowsHover)
                throw new System.Exception(string.Format(
                    "{0} cannot filter hover candidates - it does not allow hover at all.", GetType().Name));
        }

        public IOrder CreateOrder(MapElement orderExecutor, OrderActionArgs args)
        {
            if (orderExecutor == null)
                throw new System.ArgumentException("\"MapElement orderExecutor\" argument cannot be NULL.");
            if (!(orderExecutor is T))
                throw new System.ArgumentException(string.Format(
                    "\"MapElement orderExecutor\" is of type {0}, but it is used in OrderAction<{1}>. \n" +
                    "Provide argument of type {1} instead", orderExecutor.GetType(), typeof(T)));

            return CreateOrder((T)orderExecutor, args);
        }

        public abstract IOrder CreateOrder(T orderExecutor, OrderActionArgs args);

        protected void AssertOrderActionArgsTypeValid<U>(OrderActionArgs args)
            where U : OrderActionArgs
        {
            if (args == null)
                throw new System.ArgumentException("\"OrderActionArgs args\" argument cannot be NULL.");

            if (!(args is U))
                throw new System.ArgumentException(string.Format(
                    "Method \"CreateOrder\" in class {0} must take {1} as \"OrderActionArgs args\" argument.",
                    GetType().Name, typeof(U).Name));
        }
    }
}
