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

        public ProductionAgent(AIBrain brain, MainAgent parent)
            : base("Production", brain, parent)
        {
            requests = new List<Request>();
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
                    SendMessage(message.Sender, AIName.Ok, message);
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

                    var creators = Army.Buildings.Where(b => b.mapElementName == creatorKind.Name);
                    if (creators.Empty())
                        continue; // TODO: create factory

                    var creator = creators.SelectMin(c => c.OrderQueue.OrderCount);
                    var orderAction = creator.orderActions.OfType<UnitProductionOrderAction>().FirstOrDefault(
                        oa => oa.unit.mapElementName == unitKind.Name);
                   
                    //orderAction.GiveOrder(creator, new AIOrderActionArgs(Brain.player, place));

                    processed.Add(r);
                }
            }
            foreach (var r in processed)
                requests.Remove(r);
        }
    }
}