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
        Resource currentResource;

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
            {
                HarvestedRegion = PickHarvestedRegion();
                if (HarvestedRegion == null) return;
            }

            var currentOrder = UnitAgent.Unit.OrderQueue.CurrentOrder;
            if (!(currentOrder is HarvestOrder))
                GiveNewOrder();
            else
            {
                var harvestOrder = (HarvestOrder)currentOrder;
                if (currentResource != harvestOrder.Resource)
                {
                    currentResource = harvestOrder.Resource;
                    if (!HarvestedRegion.HasResource(currentResource))
                        GiveNewOrder();
                }
            }
        }

        ResourceRegionBatch PickHarvestedRegion()
        {
            var resRegions = Agent.Knowledge.Resources.Regions;
            if (resRegions.Empty())
            {
                Finish();
                return null;
            }
            var refineries = resourceCollector.Refineries;
            if (refineries.Empty())
                return resRegions.SelectMin(r => r.ConvexHull.GetDistanceTo(UnitAgent.Unit.Coords));
            else
            {
                Building refinery;
                var minDist = float.MaxValue;
                ResourceRegionBatch harvestedRegion = null;
                foreach (var reg in resRegions)
                    foreach (var rfn in refineries)
                    {
                        float dist;
                        reg.ConvexHull.GetPointClosestTo(rfn.Coords, out dist);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            refinery = rfn;
                            harvestedRegion = reg;
                        }
                    }
                return harvestedRegion;
            }
        }

        void GiveNewOrder()
        {
            var unit = UnitAgent.Unit;
            unit.OrderQueue.CancelAll();
            var resInfo = HarvestedRegion.Resources.FirstOrDefault(ri => ri.Resource != null);
            if (resInfo != null)
            {
                harvestOrderAction.GiveOrder(unit,
                    new AIOrderActionArgs(Agent.Player, resInfo.Resource));
                currentResource = resInfo.Resource;
            }
        }
    }
}