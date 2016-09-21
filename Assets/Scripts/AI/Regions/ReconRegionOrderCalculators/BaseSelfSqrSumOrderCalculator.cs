using MechWars.AI.Agents.Goals;

namespace MechWars.AI.Regions.ReconRegionOrderCalculators
{
    public class BaseSelfSqrSumOrderCalculator : ReconRegionOrderCalculator
    {
        public override float Calculate(CoarseReconGoal goal, ReconRegionBatch region)
        {
            var baseReg = goal.Agent.Knowledge.AllyBase.BaseRegion;
            var delta = region.ConvexHull.Center - baseReg.ConvexHull.Center;
            var distToBase = delta.magnitude;
            var deltaToSelf = region.ConvexHull.Center - goal.UnitAgent.Unit.Coords;
            var distToSelf = deltaToSelf.magnitude;
            return distToBase * distToBase + distToSelf * distToSelf;
        }
    }
}