using MechWars.AI.Regions;
using MechWars.MapElements;
using MechWars.MapElements.Orders;
using MechWars.MapElements.Orders.Actions;
using MechWars.Utils;
using System.Linq;

namespace MechWars.AI.Agents.Goals
{
    public class HarvestGoal : UnitAgentGoal
    {
        ResourceCollectorAgent resourceCollector;
        OrderAction harvestOrderAction;

        public ResourceRegionBatch HarvestedRegion { get; private set; }

        public HarvestGoal(UnitAgent unitAgent, ResourceCollectorAgent resourceCollector)
            : base("Harvest", unitAgent)
        {
            this.resourceCollector = resourceCollector;
        }

        protected override void OnStart()
        {
            harvestOrderAction = UnitAgent.Unit.orderActions.FirstOrDefault(oa => oa is HarvestResourceOrderAction);
            if (harvestOrderAction == null)
                Cancel();
        }

        protected override void OnUpdate()
        {
            if (HarvestedRegion != null && HarvestedRegion.RegionEmpty)
                HarvestedRegion = null;
            if (HarvestedRegion == null)
                PickHarvestedRegion();
            if (HarvestedRegion == null)
                return;

            var unit = UnitAgent.Unit;
            bool giveNewOrder = false;
            if (unit.OrderQueue.CurrentOrder is HarvestOrder)
            {
                var harvestOrder = (HarvestOrder)unit.OrderQueue.CurrentOrder;
                if (HarvestedRegion.Resources.None(ri =>
                {
                    return ri.Resource == harvestOrder.Resource;
                }))
                    giveNewOrder = true;
            }
            else giveNewOrder = true;

            if (giveNewOrder)
            {
                unit.OrderQueue.CancelAll();
                var resInfo = HarvestedRegion.Resources.FirstOrDefault(ri =>
                {
                    return ri.Resource != null;
                });
                if (resInfo != null)
                {
                    harvestOrderAction.GiveOrder(unit,
                        new AIOrderActionArgs(Agent.Player, resInfo.Resource));
                }
            }
        }

        void PickHarvestedRegion()
        {
            var resRegions = Agent.Knowledge.Resources.Regions;
            if (resRegions.Empty())
            {
                Finish();
                return;
            }
            var refineries = resourceCollector.Refineries;
            if (refineries.Empty())
                HarvestedRegion = resRegions.SelectMin(r => r.ConvexHull.GetDistanceTo(UnitAgent.Unit.Coords));
            else
            {
                Building refinery;
                var minDist = float.MaxValue;
                foreach (var reg in resRegions)
                    foreach (var rfn in refineries)
                    {
                        float dist;
                        reg.ConvexHull.GetPointClosestTo(rfn.Coords, out dist);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            refinery = rfn;
                            HarvestedRegion = reg;
                        }
                    }
            }
        }
    }
}