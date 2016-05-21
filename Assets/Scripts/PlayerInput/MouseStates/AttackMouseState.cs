using System.Collections.Generic;
using MechWars.MapElements;
using UnityEngine;
using MechWars.MapElements.Orders.Actions;

namespace MechWars.PlayerInput.MouseStates
{
    public class AttackMouseState : MouseState
    {
        public override Color FramesColor { get { return Color.red; } }

        public AttackMouseState(MouseStateController stateController)
            : base(stateController)
        {
        }

        public override void FilterHoverCandidates(HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.Attack(candidates);
        }
        
        public override void Handle()
        {
            if (StateController.LeftActionTriggered)
                GiveOrdersIfPossible(
                    typeof(FollowAttackOrderAction),
                    typeof(StandAttackOrderAction),
                    typeof(MoveOrderAction));
        }
    }
}
