using MechWars.Human;
using MechWars.MapElements;
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
                else if (inputController.Player.Army != null)
                    HandleAutomaticOrder(inputController);

        }

        void HandleAutomaticOrder(InputController inputController)
        {
            var hovered = inputController.HoverController.HoveredMapElements;
            if (hovered.HasAtLeast(2))
                throw new System.Exception("The game reached an invalid state: " +
                    "handling automatic order, while there are more than 1 hovered MapElements.");
            var mapElement = hovered.FirstOrDefault();

            var selected = inputController.SelectionMonitor.SelectedMapElements;

            bool handled;
            if (mapElement != null)
            {
                if (mapElement.army != null &&
                    mapElement.army != inputController.Player.Army &&
                    mapElement.CanBeAttacked)
                {
                    //foreach (var me in selected)
                    //{
                    //    if (me.)
                    //}
                }
                //    selected.Units.Where(u => u.CanAttack).GiveOrder(u => new FollowAttackOrder(u, mapElement.AsEnumerable()));
                //else if (mapElement != null && mapElement is Resource)
                //{
                //    selected.Units.Where(u => u.canCollect).GiveOrder(u => new HarvestOrder(u, (Resource)mapElement));
                //    selected.Units.Where(u => !u.canCollect).GiveOrder(u => new MoveOrder(u, dest.Value));
                //}
                //else if (mapElement != null && mapElement is Building && (mapElement as Building).isResourceDeposit)
                //{
                //    selected.Units.Where(u => u.canCollect).GiveOrder(u => new HarvestOrder(u, (Building)mapElement));
                //    selected.Units.Where(u => !u.canCollect).GiveOrder(u => new MoveOrder(u, dest.Value));
                //}
                handled = true;
            }
            //else if (dest != null)
            //    selected.Units.GiveOrder(u => new MoveOrder(u, dest.Value));
        }
    }
}
