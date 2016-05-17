using MechWars.Human;
using MechWars.MapElements;
using MechWars.MapElements.Orders.Actions;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.PlayerInput.MouseStates
{
    public class DefaultMouseState : MouseState
    {
        public DefaultMouseState(InputController inputController)
            : base(inputController)
        {
        }

        public override void FilterHoverCandidates(HumanPlayer player, HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.Select(player, candidates);
        }

        bool leftDown;
        public override void Handle()
        {
            if (InputController.Mouse.MouseStateLeft.IsDown)
                leftDown = true;

            if (InputController.Mouse.MouseStateRight.IsDown)
                if (leftDown) leftDown = false;
                else if (InputController.HumanPlayer.Army != null && InputController.Mouse.MapRaycast.Coords.HasValue)
                    HandleAutomaticOrder();

            var hovered = InputController.HoverController.HoveredMapElements;

            if (leftDown && InputController.Mouse.MouseStateLeft.IsUp)
            {
                InputController.SelectionMonitor.SelectNew(hovered);
                leftDown = false;
            }
        }

        void HandleAutomaticOrder()
        {
            var hovered = InputController.HoverController.HoveredMapElements;
            if (hovered.HasAtLeast(2))
                throw new System.Exception("The game reached an invalid state: " +
                    "handling automatic order, while there are more than 1 hovered MapElements.");
            var mapElement = hovered.FirstOrDefault();

            bool handled = false;
            if (mapElement != null)
            {
                handled = true;
                if (mapElement.CanBeAttacked && mapElement.army != null && mapElement.army != InputController.HumanPlayer.Army)
                    GiveOrdersIfPossible(
                        typeof(FollowAttackOrderAction),
                        typeof(StandAttackOrderAction),
                        typeof(MoveOrderAction));
                else if (mapElement is Resource)
                    GiveOrdersIfPossible(
                        typeof(HarvestResourceOrderAction),
                        typeof(MoveOrderAction));
                else if (mapElement is Building && (mapElement as Building).isResourceDeposit)
                    GiveOrdersIfPossible(
                        typeof(HarvestRefineryOrderAction),
                        typeof(MoveOrderAction));
                else handled = false;
            }
            if (!handled)
            {
                GiveOrdersIfPossible(typeof(MoveOrderAction));
                handled = true;
            }
        }
    }
}
