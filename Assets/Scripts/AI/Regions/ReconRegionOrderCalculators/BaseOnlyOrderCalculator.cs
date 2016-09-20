using MechWars.AI.Agents.Goals;

namespace MechWars.AI.Regions.ReconRegionOrderCalculators
{
    public class BaseOnlyOrderCalculator : ReconRegionOrderCalculator
    {
        public override float Calculate(CoarseReconGoal goal, ReconRegionBatch region)
        {
            var baseReg = goal.Agent.Knowledge.AllyBase.BaseRegion;
            var distToBase = (region.ConvexHull.Center - baseReg.ConvexHull.Center).magnitude;
            return distToBase;
        }
    }
}