using MechWars.Human;
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

        public virtual bool AllowsMultiExecutor { get { return true; } }
        public virtual bool AllowsMultiTarget { get { return false; } }
        public virtual bool AllowsHover { get { return false; } }
        public virtual bool IsAttack { get { return false; } }
        public virtual bool IsEscort { get { return false; } }
        public virtual bool CanBeCarried { get { return false; } }
        public virtual bool IsSequential { get { return false; } }

        public virtual void FilterHoverCandidates(HumanPlayer player, HashSet<MapElement> candidates)
        {
            if (!AllowsHover)
                throw new System.Exception(string.Format(
                    "{0} cannot filter hover candidates - it does not allow hover at all.", GetType().Name));
        }

        public virtual bool CanCreateOrder(ICanCreateOrderArgs args)
        {
            return true;
        }

        public abstract Order CreateOrder(MapElement orderExecutor, OrderActionArgs args);

        public virtual OrderActionArgs CreateArgs(InputController inputController)
        {
            return new OrderActionArgs(
                inputController.Mouse.MapRaycast.Coords.Value,
                inputController.HoverController.HoveredMapElements);
        }

        protected IEnumerable<T> TryExtractTargetsArg<T>(OrderActionArgs args)
            where T : MapElement
        {

            var wrongTypeTargets = args.Targets.Where(t => !(t is T));
            if (!wrongTypeTargets.Empty())
                throw new System.Exception(string.Format(
                    "OrderActionArgs.Target must be a {0} for {1}. Following targets were of wrong type: {2}.",
                    typeof(T).Name, GetType().Name, wrongTypeTargets.ToDebugMessage()));
            return args.Targets.Cast<T>();
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
