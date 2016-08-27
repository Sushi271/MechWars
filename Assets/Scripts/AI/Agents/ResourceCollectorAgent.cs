using MechWars.MapElements;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;

namespace MechWars.AI.Agents
{
    public class ResourceCollectorAgent : Agent
    {
        public HashSet<UnitAgent> Harvesters { get; private set; }
        public HashSet<Building> Refineries { get; private set; }

        bool refineryOnTheWay;

        public ResourceCollectorAgent(AIBrain brain, MainAgent parent)
            : base("ResourceCollector", brain, parent)
        {
            Harvesters = new HashSet<UnitAgent>();
            Refineries = new HashSet<Building>();
        }

        protected override void OnUpdate()
        {
            ProcessMessages();

            PerformEvery(1, UpdateRefineries);

            if (Refineries.Count == 0)
            {
                if (!refineryOnTheWay)
                    PerformEvery(1, RequestForRefineryConstruction);
            }
            else refineryOnTheWay = false;

            var allHarvesters = Knowledge.UnitAgents[AIName.Harvester];
            if (Harvesters.Count == 0)
            {
                // TODO: Request for production of Harvesters
            }

            PerformEvery(1, TryRequestForResourceSearch);

            var resRegions = Knowledge.Resources.Regions;
            if (!resRegions.Empty() && !Harvesters.Empty())
            {

            }
        }

        void ProcessMessages()
        {
            Message message;
            while ((message = ReceiveMessage()) != null)
            {
                if (message.Name == AIName.Ok)
                {
                    var innerMessage = message.InnerMessage;
                    if (innerMessage.Name == AIName.ConstructMeBuilding)
                    {
                        StopPerform(RequestForRefineryConstruction);
                        refineryOnTheWay = true;
                    }
                    else if (innerMessage.Name == AIName.FindMeResources)
                        StopPerform(TryRequestForResourceSearch);
                }
            }
        }

        void UpdateRefineries()
        {
            Refineries.RemoveWhere(r => r.Dying);            
            Refineries.UnionWith(Army.Buildings.Where(b => b.mapElementName == AIName.Refinery));
        }

        void RequestForRefineryConstruction()
        {
            SendMessage(Construction, AIName.ConstructMeBuilding, "0", AIName.Refinery);
        }

        void TryRequestForResourceSearch()
        {
            var resRegions = Knowledge.Resources.Regions;
            if (resRegions.Empty())
                SendMessage(Recon, AIName.FindMeResources, "0");
            else if (resRegions.HasAtLeast(1))
                SendMessage(Recon, AIName.FindMeResources, "1");
            else if (resRegions.HasAtLeast(3))
                SendMessage(Recon, AIName.FindMeResources, "2");
        }

        void TryRequestForHarvesterProduction()
        {
            var allHarvesters = Knowledge.UnitAgents[AIName.Harvester];
            if (allHarvesters.Empty())
                SendMessage(Construction, AIName.ProduceMeUnits, "0", AIName.Harvester);
            else if (allHarvesters.HasAtLeast(3))
                SendMessage(Construction, AIName.ProduceMeUnits, "1", AIName.Harvester);
            else if (allHarvesters.HasAtLeast(10))
                SendMessage(Construction, AIName.ProduceMeUnits, "2", AIName.Harvester);
        }
    }
}