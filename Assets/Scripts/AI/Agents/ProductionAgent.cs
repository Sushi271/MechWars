using MechWars.MapElements;
using MechWars.MapElements.Orders;
using MechWars.MapElements.Orders.Actions;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.AI.Agents
{
    public class ProductionAgent : Agent
    {
        List<Request> requests;
        HashSet<Order> givenOrders;

        public ProductionAgent(AIBrain brain, MainAgent parent)
            : base("Production", brain, parent)
        {
            requests = new List<Request>();
            givenOrders = new HashSet<Order>();
        }

        public int GetGivenOrdersCountOfKind(MapElementKind unitKind)
        {
            return givenOrders.OfType<UnitProductionOrder>().Count(o =>
                o.ProducedUnit.mapElementName == unitKind.Name);
        }

        protected override void OnUpdate()
        {
            ProcessMessages();
            ProcessRequests();
        }

        void ProcessMessages()
        {
            Message message;
            while ((message = ReceiveMessage()) != null)
            {
                if (message.Name == AIName.ProduceMeUnits)
                {
                    requests.Add(new Request(message.Sender, message.Name, int.Parse(message.Arguments[0]), message));
                }
            }
            requests.Sort((r1, r2) => r1.Priority.CompareTo(r2.Priority));
        }

        void ProcessRequests()
        {
            var processed = new List<Request>();
            foreach (var r in requests)
            {
                if (r.Name == AIName.ProduceMeUnits)
                {
                    var unitName = r.InnerMessage.Arguments[1];
                    var unitKind = Knowledge.MapElementKinds[unitName];
                    var creationMethod = Knowledge.CreationMethods[unitKind];
                    var creatorKind = creationMethod.Creator;
                    var cost = creationMethod.Cost;
                    var time = creationMethod.Time;
                    // requirements are none, so we can ignore them

                    bool dontFinish = false;
                    Building creator = null;
                    UnitProductionOrderAction orderAction = null;

                    var completeBuildings = Army.Buildings.Where(b => !b.UnderConstruction);
                    var creators = completeBuildings.Where(b => b.mapElementName == creatorKind.Name);
                    if (creators.Empty())
                    {
                        if (!Construction.HasCurrentRequestOfKind(creatorKind) &&
                            !Construction.HasGivenOrdersOfKind(creatorKind) &&
                            !Army.Buildings.Any(_b => _b.mapElementName == creatorKind.Name))
                            SendMessage(Construction, AIName.ConstructMeBuilding, r.Priority.ToString(), creatorKind.Name);
                        dontFinish = true;
                    }
                    else
                    {
                        creator = creators.SelectMin(c => c.OrderQueue.OrderCount);
                        orderAction = creator.orderActions.OfType<UnitProductionOrderAction>().FirstOrDefault(
                            oa => oa.unit.mapElementName == unitKind.Name);
                        if (Army.resources < cost)
                        {
                            var isHarvester = unitName == AIName.Harvester;
                            var hasHarvesters = Army.Units.Any(b => b.mapElementName == AIName.Harvester);
                            if (isHarvester && !hasHarvesters)
                            {
                                processed.Add(r);
                                SendMessage(MainAgent, AIName.NoHarvestersAndNoResources);
                                dontFinish = true;
                            }
                            else SendMessage(ResourceCollector, AIName.HarvestMore);
                        }
                    }

                    if (dontFinish) continue;

                    var givenOrder = (UnitProductionOrder)orderAction.GiveOrder(creator, new AIOrderActionArgs(Brain.player));
                    if (givenOrder != null)
                    {
                        givenOrder.UnitSpawned += GivenOrder_UnitSpawned;
                        givenOrders.Add(givenOrder);
                    }

                    processed.Add(r);
                }
            }
            foreach (var r in processed)
                requests.Remove(r);
        }

        void GivenOrder_UnitSpawned(UnitProductionOrder order, Unit unit)
        {
            givenOrders.Remove(order);
            order.UnitSpawned -= GivenOrder_UnitSpawned;
        }
    }
}