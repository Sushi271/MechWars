using System.Collections.Generic;
using MechWars.MapElements;
using UnityEngine;
using MechWars.Human;
using MechWars.MapElements.Orders.Actions;

namespace MechWars.PlayerInput.MouseStates
{
    public class AttackMouseState : MouseState
    {
        static AttackMouseState instance;
        public static AttackMouseState Instance
        {
            get
            {
                if (instance == null)
                    instance = new AttackMouseState();
                return instance;
            }
        }

        public override Color FramesColor { get { return Color.red; } }

        AttackMouseState()
        {
        }

        public override void FilterHoverCandidates(HumanPlayer player, HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.Attack(player, candidates);
        }

        bool leftDown;
        public override void Handle(InputController inputController)
        {
            var hovered = inputController.HoverController.HoveredMapElements;
            
            if (inputController.Mouse.MouseStateLeft.IsDown) leftDown = true;
            if (inputController.Mouse.MouseStateRight.IsDown) leftDown = false;
            if (leftDown && inputController.Mouse.MouseStateLeft.IsUp)
            {
                GiveOrdersIfPossible(inputController, hovered,
                    typeof(FollowAttackOrderAction), typeof(StandAttackOrderAction), typeof(MoveOrderAction));
                leftDown = false;
            }
        }
    }
}
