using MechWars.AI.Regions;
using MechWars.FogOfWar;
using MechWars.MapElements.Orders;
using MechWars.MapElements.Statistics;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.AI.Agents.Goals
{
    public class CoarseReconGoal : UnitAgentGoal
    {
        HashSet<ReconRegionBatch> visited;
        public ReconRegionBatch CurrentReconRegion { get; private set; }

        MoveOrder currentMoveOrder;

        List<System.Action> Debuggg;
        
        public CoarseReconGoal(UnitAgent unitAgent)
            : base("Recon", unitAgent)
        {
            visited = new HashSet<ReconRegionBatch>();

            Debuggg = new List<System.Action>();
        }

        protected override void OnStart()
        {
            TakeNextReconRegion();
        }

        void TakeNextReconRegion()
        {
            var recRegs = Agent.Recon.AllReconRegions;
            var sortedRecRegs =
                from reg in recRegs
                where !visited.Contains(reg)
                where reg.ExplorationPercentage < Agent.Brain.coarseReconRegionPercentage
                let order1 = CalculateOrderBaseSelfSum(reg)
                let order2 = CalculateOrderBaseSelfProductAndTilesToExplore(reg)
                orderby order2
                select new { reg = reg, o1 = order1, o2 = order2 };
            Debuggg = new List<System.Action>(sortedRecRegs.Select(a =>
                new System.Action(() => Debug.LogFormat("Center: {0}\nOrder1: {1}\nOrder2: {2}",
                a.reg.ConvexHull.Center, a.o1, a.o2))));
            var otherUnitAgents = Agent.Recon.ReconUnits
                .SelectMany(kv => kv.Value.Ready)
                .Where(ua => ua != UnitAgent)
                .Where(ua => ua.CurrentGoal != null)
                .Where(ua => ((CoarseReconGoal)ua.CurrentGoal).CurrentReconRegion != null);
            CurrentReconRegion = sortedRecRegs.Select(reg => reg.reg).FirstOrDefault(
                reg => !otherUnitAgents.Any(
                    ua => ((CoarseReconGoal)ua.CurrentGoal).CurrentReconRegion == reg));
            if (CurrentReconRegion == null)
                Finish();
        }

        protected override void OnUpdate()
        {
            if (CurrentReconRegion == null) return;

            if (CurrentReconRegion.ExplorationPercentage >= Agent.Brain.coarseReconRegionPercentage)
                currentMoveOrder.Stop();
            if (currentMoveOrder != null && currentMoveOrder.InFinalState)
                currentMoveOrder = null;

            if (currentMoveOrder == null)
            {
                visited.Add(CurrentReconRegion);
                TakeNextReconRegion();
            }

            var regCenter = CurrentReconRegion.ConvexHull.Center.Round();
            var u = UnitAgent.Unit;
            if (u.OrderQueue.CurrentOrder == null ||
                !(u.OrderQueue.CurrentOrder is MoveOrder))
            {
                currentMoveOrder = new MoveOrder(u, regCenter);
                u.OrderQueue.Give(currentMoveOrder);
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                foreach (var a in Debuggg)
                    a();
            }
        }

        float CalculateOrderBaseOnly(ReconRegionBatch region)
        {
            var baseReg = Agent.Knowledge.AllyBase.BaseRegion;
            var distToBase = (region.ConvexHull.Center - baseReg.ConvexHull.Center).magnitude;
            return distToBase;
        }

        float CalculateOrderBaseSelfSum(ReconRegionBatch region)
        {
            var distToBase = CalculateOrderBaseOnly(region);
            var distToSelf = (region.ConvexHull.Center - UnitAgent.Unit.Coords).magnitude;
            return distToBase + distToSelf;
        }

        float CalculateOrderBaseSelfSqrSum(ReconRegionBatch region)
        {
            var distToBase = CalculateOrderBaseOnly(region);
            var distToSelf = (region.ConvexHull.Center - UnitAgent.Unit.Coords).magnitude;
            return distToBase * distToBase + distToSelf * distToSelf;
        }

        float CalculateOrderBaseSelfProductAndTilesToExplore(ReconRegionBatch region)
        {
            var orderBaseSelfProduct = CalculateOrderBaseSelfSum(region);

            var uShape = UnitAgent.Unit.Shape;
            var radiusStat = UnitAgent.Unit.Stats[StatNames.ViewRange];
            if (radiusStat == null) return orderBaseSelfProduct;
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
                        Agent.Army.VisibilityTable[rx, ry] == Visibility.Unknown)
                        tilesToExploreInShape++;
                }
            var tilesToExploreRatio = (float)tilesToExploreInShape / tilesInShape;

            return orderBaseSelfProduct * Mathf.Pow(tilesToExploreRatio, 0.25f);
        }
    }
}