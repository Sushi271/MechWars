using MechWars.MapElements;
using MechWars.MapElements.Orders.Actions;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;

namespace MechWars.PlayerInput.MouseStates
{
    public class DefaultMouseState : MouseState
    {
        public DefaultMouseState(MouseStateController stateController)
            : base(stateController)
        {
        }

        public override void FilterHoverCandidates(HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.Select(candidates);
        }
        
        public override void Handle()
        {
            if (StateController.RightActionTriggered)
            {
                if (Globals.HumanArmy != null && InputController.Mouse.MapRaycast.Coords.HasValue)
                    HandleAutomaticOrder();
            }
            else if (StateController.LeftActionTriggered)
            {
                var hovered = InputController.HoverController.HoveredMapElements;
                InputController.SelectionMonitor.SelectNew(hovered);
            }
        }

        void HandleAutomaticOrder()
        {
            var hovered = InputController.HoverController.HoveredMapElements;
            if (hovered.HasAtLeast(2))
                throw new System.Exception(string.Format("The game reached an invalid state: " +
                    "handling automatic order, while there are more than 1 hovered MapElements. " +
                    "List of hovered MapElements: {0}", hovered.ToDebugMessage()));
            var mapElement = hovered.FirstOrDefault();

            bool handled = false;
            if (mapElement != null)
            {
                handled = true;
                if (mapElement.CanBeAttacked && mapElement.Army != null && mapElement.Army != Globals.HumanArmy)
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
