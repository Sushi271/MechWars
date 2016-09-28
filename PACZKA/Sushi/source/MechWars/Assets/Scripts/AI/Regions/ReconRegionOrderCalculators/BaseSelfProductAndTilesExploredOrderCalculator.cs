using MechWars.AI.Agents.Goals;
using MechWars.FogOfWar;
using MechWars.MapElements.Statistics;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.AI.Regions.ReconRegionOrderCalculators
{
    public class BaseSelfProductAndTilesExploredOrderCalculator : ReconRegionOrderCalculator
    {
        public override float Calculate(CoarseReconGoal goal, ReconRegionBatch region)
        {
            var baseReg = goal.Agent.Knowledge.AllyBase.BaseRegion;
            var dToBase = region.ConvexHull.Center - baseReg.ConvexHull.Center;
            var distToBase = dToBase.magnitude;
            var dToSelf = region.ConvexHull.Center - goal.UnitAgent.Unit.Coords;
            var distToSelf = dToSelf.magnitude;
            var baseSelfProduct = distToBase * distToSelf;

            var uShape = goal.UnitAgent.Unit.Shape;
            var radiusStat = goal.UnitAgent.Unit.Stats[StatNames.ViewRange];
            if (radiusStat == null) return baseSelfProduct;
            var radius = radiusStat.Value;

            var losShape = Globals.LOSShapeDatabase[radius, uShape];
            var center = region.ConvexHull.Center.Round();
            int x = center.X;
            int y = center.Y;

            int tilesInShape = 0;
            int tilesExploredInShape = 0;
            for (int rx = losShape.GetXMin(x), i = 0; rx <= losShape.GetXMax(x); rx++, i++)
                for (int ry = losShape.GetYMin(y), j = 0; ry <= losShape.GetYMax(y); ry++, j++)
                {
                    if (!losShape[i, j]) continue;
                    tilesInShape++;
                    if (Globals.Map.IsInBounds(rx, ry) &&
                        goal.Agent.Army.VisibilityTable[rx, ry] != Visibility.Unknown)
                        tilesExploredInShape++;
                }
            var explorationRatio = (float)tilesExploredInShape / tilesInShape;

            float minVal = 0.1f;
            float power = 0.78f;

            var factor = minVal + (explorationRatio * (1 - minVal));
            return baseSelfProduct * Mathf.Pow(factor, power);
        }
    }
}