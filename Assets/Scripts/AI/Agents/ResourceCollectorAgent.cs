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

            PerformEvery(1, RequestForHarvesterProduction);
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
                else if (message.Name == AIName.HandMeOnUnits)
                {
                    int id = int.Parse(message.Arguments[0]);
                    var harv = Harvesters.FirstOrDefault(h => h.Id == id);
                    if (harv != null && message.Sender.MakeSureIfHandOn(harv))
                    {
                        Harvesters.Remove(harv);
                        harv.CurrentGoal.Cancel();
                        harv.HandOn(this, message.Sender);
                    }
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
            else if (resRegions.HasNoMoreThan(1))
                SendMessage(Recon, AIName.FindMeResources, "1");
            else if (resRegions.HasNoMoreThan(3))
                SendMessage(Recon, AIName.FindMeResources, "2");
        }
        
        void RequestForHarvesterProduction()
        {
            var allHarvesters = Knowledge.UnitAgents[AIName.Harvester];
            int harvsCount = allHarvesters.Count;
            int harvsInProgress = Production.GetGivenOrdersCountOfKind(Knowledge.MapElementKinds[AIName.Harvester]);
            int harvsNeededNow = HarvestersOverTime();
            int harvsShortage = harvsNeededNow - (harvsCount + harvsInProgress);
            for (int i = 0; i < harvsShortage; i++)
                SendMessage(Production, AIName.ProduceMeUnits, "0", AIName.Harvester);
        }

        int HarvestersOverTime()
        {
            return (int)(Time.time / 60) + 3;
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