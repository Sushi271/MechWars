using MechWars.PlayerInput;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.MapElements.Orders.Actions
{
    public abstract class OrderAction : MonoBehaviour, IMouseBehaviourDeterminant
    {
        public Color framesColor = Color.black;
        public Color FramesColor { get { return framesColor; } }
        public Color HoverBoxColor { get { return framesColor; } }

        public virtual bool AllowsMultiExecutor { get { return true; } }
        public virtual bool AllowsMultiTarget { get { return false; } }
        public virtual bool AllowsHover { get { return false; } }
        public virtual bool IsAttack { get { return false; } }
        public virtual bool IsEscort { get { return false; } }
        public virtual bool CanBeCarried { get { return false; } }
        public virtual bool IsSequential { get { return false; } }

        public virtual void FilterHoverCandidates(HashSet<MapElement> candidates)
        {
            if (!AllowsHover)
                throw new System.Exception(string.Format(
                    "{0} cannot filter hover candidates - it does not allow hover at all.", GetType().Name));
        }

        public bool GiveOrder(MapElement orderExecutor, IOrderActionArgs orderActionArgs)
        {
            if (orderExecutor.OrderQueue.Enabled && CanCreateOrder(orderActionArgs))
            {
                orderExecutor.OrderQueue.Give(CreateOrder(orderExecutor, orderActionArgs));
                return true;
            }
            return false;
        }

        protected virtual bool CanCreateOrder(IOrderActionArgs orderActionArgs)
        {
            return true;
        }

        protected abstract Order CreateOrder(MapElement orderExecutor, IOrderActionArgs orderActionArgs);

        protected IEnumerable<T> TryExtractTargetsArg<T>(IOrderActionArgs orderActionArgs)
            where T : MapElement
        {

            var wrongTypeTargets = orderActionArgs.Targets.Where(t => !(t is T));
            if (!wrongTypeTargets.Empty())
                throw new System.Exception(string.Format(
                    "OrderActionArgs.Target must be a {0} for {1}. Following targets were of wrong type: {2}.",
                    typeof(T).Name, GetType().Name, wrongTypeTargets.ToDebugMessage()));
            return orderActionArgs.Targets.Cast<T>();
        }

        protected void AssertOrderExecutorIs<T>(MapElement orderExecutor)
            where T : MapElement
        {
            if (!(orderExecutor is T))
                throw new System.ArgumentException(string.Format(
                    "\"MapElement orderExecutor\" must be a {0}.", typeof(T).Name));
        }

        protected void AssertOrderExecutorIs<T1, T2>(MapElement orderExecutor)
            where T1 : MapElement
            where T2 : MapElement
        {
            if (!(orderExecutor is T1) && !(orderExecutor is T2))
                throw new System.ArgumentException(string.Format(
                    "\"MapElement orderExecutor\" must be a {0}, or a {1}.", typeof(T1).Name, typeof(T2).Name));
        }
    }
}
