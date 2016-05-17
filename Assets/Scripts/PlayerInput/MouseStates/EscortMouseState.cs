using System.Collections.Generic;
using MechWars.MapElements;
using UnityEngine;
using MechWars.Human;
using MechWars.MapElements.Orders.Actions;

namespace MechWars.PlayerInput.MouseStates
{
    public class EscortMouseState : MouseState
    {
        public override Color FramesColor { get { return Color.blue; } }

        public EscortMouseState(InputController inputController)
            : base(inputController)
        {
        }

        public override void FilterHoverCandidates(HumanPlayer player, HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.Escort(player, candidates);
        }

        bool leftDown;
        public override void Handle()
        {
            if (InputController.Mouse.MouseStateLeft.IsDown) leftDown = true;
            if (InputController.Mouse.MouseStateRight.IsDown) leftDown = false;
            if (leftDown && InputController.Mouse.MouseStateLeft.IsUp)
            {
                GiveOrdersIfPossible(typeof(EscortOrderAction));
                leftDown = false;
            }
        }
    }
}
