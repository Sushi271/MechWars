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

            var hovered = inputController.HoverController.HoveredMapElements;

            if (leftDown && inputController.Mouse.MouseStateLeft.IsUp)
            {
                inputController.SelectionMonitor.SelectNew(hovered);
                leftDown = false;
            }
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
                    GiveOrdersIfPossible(inputController, mapElement.AsEnumerable(true),
                        typeof(FollowAttackOrderAction), typeof(StandAttackOrderAction), typeof(MoveOrderAction));
                else if (mapElement is Resource)
                    GiveOrdersIfPossible(inputController, mapElement.AsEnumerable(true),
                        typeof(HarvestResourceOrderAction), typeof(MoveOrderAction));
                else if (mapElement is Building && (mapElement as Building).isResourceDeposit)
                    GiveOrdersIfPossible(inputController, mapElement.AsEnumerable(true),
                        typeof(HarvestRefineryOrderAction), typeof(MoveOrderAction));
                else handled = false;
            }
            if (!handled)
            {
                GiveOrdersIfPossible(inputController, mapElement.AsEnumerable(true), typeof(MoveOrderAction));
                handled = true;
            }
        }
    }
}
