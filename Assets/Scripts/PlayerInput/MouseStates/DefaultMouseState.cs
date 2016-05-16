using MechWars.Human;
using MechWars.MapElements;
using MechWars.MapElements.Orders.Actions;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;

namespace MechWars.PlayerInput.MouseStates
{
    public class DefaultMouseState : MouseState
    {
        static DefaultMouseState instance;
        public static DefaultMouseState Instance
        {
            get
            {
                if (instance == null)
                    instance = new DefaultMouseState();
                return instance;
            }
        }

        DefaultMouseState()
        {
        }

        public override void FilterHoverCandidates(HumanPlayer player, HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.Select(player, candidates);
        }

        bool leftDown;
        public override void Handle(InputController inputController)
        {
            if (inputController.Mouse.MouseStateLeft.IsDown)
                leftDown = true;

            if (inputController.Mouse.MouseStateRight.IsDown)
                if (leftDown) leftDown = false;
                else if (inputController.Player.Army != null && inputController.MapRaycast.Coords.HasValue)
                    HandleAutomaticOrder(inputController);

            if (leftDown && inputController.Mouse.MouseStateLeft.IsUp)
                inputController.SelectionMonitor.SelectNew(inputController.HoverController.HoveredMapElements);
        }

        void HandleAutomaticOrder(InputController inputController)
        {
            var hovered = inputController.HoverController.HoveredMapElements;
            if (hovered.HasAtLeast(2))
                throw new System.Exception("The game reached an invalid state: " +
                    "handling automatic order, while there are more than 1 hovered MapElements.");
            var mapElement = hovered.FirstOrDefault();

            bool handled = false;
            if (mapElement != null)
            {
                handled = true;
                if (mapElement.CanBeAttacked && mapElement.army != null && mapElement.army != inputController.Player.Army)
                    GiveOrdersIfPossible(inputController, mapElement,
                        typeof(FollowAttackOrderAction), typeof(StandAttackOrderAction));
                else if (mapElement is Resource)
                    GiveOrdersIfPossible(inputController, mapElement,
                        typeof(HarvestResourceOrderAction), typeof(MoveOrderAction));
                else if (mapElement is Building && (mapElement as Building).isResourceDeposit)
                    GiveOrdersIfPossible(inputController, mapElement,
                        typeof(HarvestRefineryOrderAction), typeof(MoveOrderAction));
                else handled = false;
            }
            if (!handled)
            {
                GiveOrdersIfPossible(inputController, mapElement, typeof(MoveOrderAction));
                handled = true;
            }
        }

        void GiveOrdersIfPossible(InputController inputController, MapElement target, params System.Type[] types)
        {
            var args = new OrderActionArgs(inputController.MapRaycast.Coords.Value, target.AsEnumerable(true));
            var selected = inputController.SelectionMonitor.SelectedMapElements;
            foreach (var me in selected)
            {
                if (me.army != inputController.Player.Army) continue;

                var result = me.orderActions.FirstOrAnother(types.Select(
                    t => new System.Func<OrderAction, bool>(oa => oa.GetType() == t)).ToArray());
                if (!result.Found) continue;

                var orderAction = result.Result;
                if (me.OrderExecutor.Enabled)
                    me.OrderExecutor.Give(orderAction.CreateOrder(me, args));
            }
        }
    }
}
