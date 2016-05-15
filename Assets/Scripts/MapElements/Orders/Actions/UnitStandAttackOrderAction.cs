using MechWars.Human;
using MechWars.MapElements.Orders.Actions.Args;
using MechWars.PlayerInput;
using System.Collections.Generic;
using System;

namespace MechWars.MapElements.Orders.Actions
{
    public class UnitStandAttackOrderAction : OrderAction<Unit>
    {
        public override bool AllowsMultiTarget { get { return true; } }
        public override bool AllowsHover { get { return true; } }
        public override bool IsAttack { get { return true; } }

        public override void FilterHoverCandidates(HumanPlayer player, HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.Attack(player, candidates);
        }

        public override IOrder CreateOrder(Unit orderExecutor, OrderActionArgs args)
        {
            AssertOrderActionArgsTypeValid<TargetOrderActionArgs>(args);
            return new StandAttackOrder(orderExecutor,
                (MapElement)args[TargetOrderActionArgs.TargetArgName].Value);
        }
    }
}