using MechWars.AI.Agents.Goals;
using MechWars.FogOfWar;
using MechWars.MapElements.Statistics;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.AI.Regions.ReconRegionOrderCalculators
{
    public class BaseSelfProductAndTilesToExploreOrderCalculator : ReconRegionOrderCalculator
    {
        public override float Calculate(CoarseReconGoal goal, ReconRegionBatch region)
        {
            var baseReg = goal.Agent.Knowledge.AllyBase.BaseRegion;
            var distToBase = (region.ConvexHull.Center - baseReg.ConvexHull.Center).magnitude;
            var distToSelf = (region.ConvexHull.Center - goal.UnitAgent.Unit.Coords).magnitude;
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
            int tilesToExploreInShape = 0;
            for (int rx = losShape.GetXMin(x), i = 0; rx <= losShape.GetXMax(x); rx++, i++)
                for (int ry = losShape.GetYMin(y), j = 0; ry <= losShape.GetYMax(y); ry++, j++)
                {
                    if (!losShape[i, j]) continue;
                    tilesInShape++;
                    if (Globals.Map.IsInBounds(rx, ry) &&
                        goal.Agent.Army.VisibilityTable[rx, ry] == Visibility.Unknown)
                        tilesToExploreInShape++;
                }
            var tilesToExploreRatio = (float)tilesToExploreInShape / tilesInShape;

            return baseSelfProduct * Mathf.Pow(tilesToExploreRatio, 0.25f);
        }
    }
}