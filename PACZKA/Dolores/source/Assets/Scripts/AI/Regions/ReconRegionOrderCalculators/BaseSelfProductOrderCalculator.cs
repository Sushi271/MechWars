using MechWars.AI.Agents.Goals;

namespace MechWars.AI.Regions.ReconRegionOrderCalculators
{
    public class BaseSelfProductOrderCalculator : ReconRegionOrderCalculator
    {
        public override float Calculate(CoarseReconGoal goal, ReconRegionBatch region)
        {
            var baseReg = goal.Agent.Knowledge.AllyBase.BaseRegion;
            var dToBase = region.ConvexHull.Center - baseReg.ConvexHull.Center;
            var distToBase = dToBase.magnitude;
            var dToSelf = region.ConvexHull.Center - goal.UnitAgent.Unit.Coords;
            var distToSelf = dToSelf.magnitude;
            return distToBase * distToSelf;
        }
    }
}