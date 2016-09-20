using MechWars.AI.Regions;
using MechWars.MapElements.Orders;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;

namespace MechWars.AI.Agents.Goals
{
    public class CoarseReconGoal : UnitAgentGoal
    {
        HashSet<ReconRegionBatch> visited;
        public ReconRegionBatch CurrentReconRegion { get; private set; }

        MoveOrder currentMoveOrder;
        
        public CoarseReconGoal(UnitAgent unitAgent)
            : base("Recon", unitAgent)
        {
            visited = new HashSet<ReconRegionBatch>();
        }

        protected override void OnStart()
        {
            TakeNextReconRegion();
        }

        protected override void OnUpdate()
        {
            if (CurrentReconRegion == null) return;

            if (currentMoveOrder != null && 
                CurrentReconRegion.ExplorationPercentage >= Agent.Brain.coarseReconPercentage)
                currentMoveOrder.Stop();
            if (currentMoveOrder != null && currentMoveOrder.InFinalState)
                currentMoveOrder = null;

            if (currentMoveOrder == null)
            {
                visited.Add(CurrentReconRegion);
                TakeNextReconRegion();

                if (CurrentReconRegion == null) return;
            }

            var regCenter = CurrentReconRegion.ConvexHull.Center.Round();
            var u = UnitAgent.Unit;
            if (u.OrderQueue.CurrentOrder == null ||
                !(u.OrderQueue.CurrentOrder is MoveOrder))
            {
                currentMoveOrder = new MoveOrder(u, regCenter);
                u.OrderQueue.Give(currentMoveOrder);
            }
        }

        void TakeNextReconRegion()
        {
            var orderCalculator = Agent.Brain.reconRegionOrderCalculator;

            var recRegs = Agent.Recon.AllReconRegions;
            var sortedRecRegs =
                from reg in recRegs
                where !visited.Contains(reg)
                where reg.ExplorationPercentage < Agent.Brain.coarseReconPercentage
                orderby orderCalculator == null ? 0 : orderCalculator.Calculate(this, reg)
                select reg;
            var otherUnitAgents = Agent.Recon.ReconUnits
                .SelectMany(kv => kv.Value.Ready)
                .Where(ua => ua != UnitAgent)
                .Where(ua => ua.CurrentGoal != null)
                .Where(ua => ((CoarseReconGoal)ua.CurrentGoal).CurrentReconRegion != null);
            CurrentReconRegion = sortedRecRegs.FirstOrDefault(
                reg => !otherUnitAgents.Any(
                    ua => ((CoarseReconGoal)ua.CurrentGoal).CurrentReconRegion == reg));
            if (CurrentReconRegion == null)
                Finish();
        }
    }
}