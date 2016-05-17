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
        public static AttackMouseState GetInstance(InputController inputController)
        {
            if (instance == null)
                instance = new AttackMouseState(inputController);
            return instance;
        }

        public override Color FramesColor { get { return Color.red; } }

        AttackMouseState(InputController inputController)
            : base(inputController)
        {
        }

        public override void FilterHoverCandidates(HumanPlayer player, HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.Attack(player, candidates);
        }

        bool leftDown;
        public override void Handle()
        {
            var hovered = InputController.HoverController.HoveredMapElements;
            
            if (InputController.Mouse.MouseStateLeft.IsDown) leftDown = true;
            if (InputController.Mouse.MouseStateRight.IsDown) leftDown = false;
            if (leftDown && InputController.Mouse.MouseStateLeft.IsUp)
            {
                GiveOrdersIfPossible(hovered,
                    typeof(FollowAttackOrderAction), typeof(StandAttackOrderAction), typeof(MoveOrderAction));
                leftDown = false;
            }
        }
    }
}
