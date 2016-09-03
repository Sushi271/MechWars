using MechWars.AI.Agents.Goals;
using MechWars.MapElements;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.AI.Agents
{
    public class ResourceCollectorAgent : Agent
    {
        public HashSet<UnitAgent> Harvesters { get; private set; }
        public HashSet<Building> Refineries { get; private set; }

        bool refineryOnTheWay;

        LerpFunc2 harvestingImportanceFunction;

        public float HarvestingImportance { get; private set; }

        public ResourceCollectorAgent(AIBrain brain, MainAgent parent)
            : base("ResourceCollector", brain, parent)
        {
            Harvesters = new HashSet<UnitAgent>();
            Refineries = new HashSet<Building>();
        }

        protected override void OnStart()
        {
            ReadHarvestingImportanceFunction();
        }

        void ReadHarvestingImportanceFunction()
        {
            var txt = Brain.harvestingImportanceFunction;
            if (txt == null) return;

            harvestingImportanceFunction = new LerpFunc2(txt.bytes);
            if (harvestingImportanceFunction.Invalid)
            {
                Debug.LogError("Failed to read harvestingImportanceFunction.");
                harvestingImportanceFunction = null;
            }
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

            PerformEvery(1, TryRequestForHarvesterProduction);
            PerformEvery(1, TryRequestForResourceSearch);

            HarvestingImportance = CalcHarvestingImportance();
            foreach (var h in Harvesters)
                if (h.CurrentGoal is HarvestGoal)
                    h.CurrentGoal.Importance = HarvestingImportance;

            var resRegions = Knowledge.Resources.Regions;
            if (!resRegions.Empty())
            {
                var freeHarvesters = Knowledge.UnitAgents[AIName.Harvester].Where(h => !h.Busy);
                foreach (var h in freeHarvesters)
                {
                    h.Take(this);
                    Harvesters.Add(h);
                    h.GiveGoal(new HarvestGoal(h, this), HarvestingImportance);
                }
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
                }
                else if (message.Name == AIName.HandMeOnUnit)
                {
                    
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
                SendMessage(Production, AIName.ProduceMeUnits, "0", AIName.Harvester);
            else if (allHarvesters.HasAtLeast(3))
                SendMessage(Production, AIName.ProduceMeUnits, "1", AIName.Harvester);
            else if (allHarvesters.HasAtLeast(10))
                SendMessage(Production, AIName.ProduceMeUnits, "2", AIName.Harvester);
        }

        float CalcHarvestingImportance()
        {
            if (harvestingImportanceFunction == null) return 0;

            float res = Army.resources;
            float harv = Harvesters.Count;
            return harvestingImportanceFunction[harv, res];
        }
    }
}