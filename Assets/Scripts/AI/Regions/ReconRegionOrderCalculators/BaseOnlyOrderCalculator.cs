using MechWars.AI.Agents.Goals;

namespace MechWars.AI.Regions.ReconRegionOrderCalculators
{
    public class BaseOnlyOrderCalculator : ReconRegionOrderCalculator
    {
        public override float Calculate(CoarseReconGoal goal, ReconRegionBatch region)
        {
            var baseReg = goal.Agent.Knowledge.AllyBase.BaseRegion;
            var delta = region.ConvexHull.Center - baseReg.ConvexHull.Center;
            var distToBase = delta.magnitude;
            return distToBase;
        }
    }
}