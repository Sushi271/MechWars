using MechWars.AI.Agents.Goals;
using MechWars.AI.Regions;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.AI.Agents
{
    public class ReconAgent : Agent
    {
        List<Request> requests;
        public Dictionary<Request, RequestUnitAgentSet> ReconUnits { get; private set; }

        public ReconRegionBatch[,] ReconRegions { get; private set; }
        public int ReconRegionsSize { get; private set; }

        public IEnumerable<ReconRegionBatch> AllReconRegions
        {
            get
            {
                for (int regY = 0; regY < ReconRegionsSize; regY++)
                    for (int regX = 0; regX < ReconRegionsSize; regX++)
                        yield return ReconRegions[regX, regY];
            }
        }
        public int AllReconRegionsCount { get { return ReconRegionsSize * ReconRegionsSize; } }

        public ReconAgent(AIBrain brain, MainAgent parent)
            : base("Recon", brain, parent)
        {
            requests = new List<Request>();
            ReconUnits = new Dictionary<Request, RequestUnitAgentSet>();
        }

        protected override void OnStart()
        {
            GenerateReconRegions();
        }

        void GenerateReconRegions()
        {
            var size = Brain.reconRegionSize;
            var mapSize = Globals.MapSettings.Size;
            var count = Mathf.CeilToInt((float)mapSize / size);
            ReconRegions = new ReconRegionBatch[count, count];
            ReconRegionsSize = count;
            for (int y = 0; y < mapSize; y++)
            {
                int regY = y / size;
                for (int x = 0; x < mapSize; x++)
                {
                    int regX = x / size;
                    var reg = ReconRegions[regX, regY];
                    if (reg == null)
                    {
                        reg = new ReconRegionBatch(Brain, true);
                        ReconRegions[regX, regY] = reg;
                    }
                    reg.Region.AddTile(x, y);
                }
            }
            for (int regY = 0; regY < count; regY++)
                for (int regX = 0; regX < count; regX++)
                {
                    ReconRegions[regX, regY].SuspendUpdateBatch = false;
                    ReconRegions[regX, regY].UpdateBatch();
                }
        }

        protected override void OnUpdate()
        {
            ProcessMessages();
            ProcessRequests();

            HandleReconUnits();
        }

        void ProcessMessages()
        {
            Message message;
            while ((message = ReceiveMessage()) != null)
            {
                if (message.Name == AIName.FindMeResources)
                {
                    var req = requests.FirstOrDefault(r => r.Name == AIName.FindMeResources);
                    if (req == null)
                    {
                        req = new Request(message.Sender, message.Name, int.Parse(message.Arguments[0]), message);
                        requests.Add(req);
                        ReconUnits.Add(req, new RequestUnitAgentSet(this, req));
                    }
                    else req.Priority = int.Parse(message.Arguments[0]);
                }
            }
            requests.Sort((r1, r2) => r1.Priority.CompareTo(r2.Priority));
        }

        void ProcessRequests()
        {
            var processed = new List<Request>();
            foreach (var r in requests)
            {
                if (r.Name == AIName.FindMeResources)
                    PerformEvery(1, ProcessFindMeResources, new ProcessFindMeResourcesArgs(r, processed));
            }
            foreach (var r in processed)
                requests.Remove(r);
        }

        bool waitingForAnyScout;
        bool waitingForNonBusyScout;
        int[] scoutsNeededByPriority = { 3, 1, 1 };
        float[] scoutsImportanceByPriority = { 0.8f, 0.6f, 0.35f };

        void ProcessFindMeResources(object args)
        {
            var concreteArgs = (ProcessFindMeResourcesArgs)args;
            var r = concreteArgs.request;
            var processed = concreteArgs.processed;

            // All available MapElementKinds of UnitAgents suitable for Scouting
            var kinds =
                from k in Knowledge.UnitAgents.Kinds
                let p = k.GetPurposeValue(AIName.Scouting)
                where p > 0
                orderby p descending
                select k;

            // Send request for production of Scouts
            if (kinds.Empty())
            {
                if (!waitingForAnyScout)
                {
                    SendMessage(Production, AIName.ProduceMeUnits, "1", AIName.Scout);
                    waitingForAnyScout = true;
                }
                return;
            }
            else waitingForAnyScout = false;

            // Determine how much scouts are needed and how important is their task
            int scoutsNeeded = scoutsNeededByPriority[r.Priority];
            float scoutsImportance = scoutsImportanceByPriority[r.Priority];

            // Get UnitAgents currently assigned to this Request and update them
            var uaSet = ReconUnits[r];
            uaSet.ReadyAgents(ua => new CoarseReconGoal(ua), scoutsImportance);
            foreach (var ua in uaSet.Ready)
                ua.CurrentGoal.Importance = scoutsImportance;

            // How much more scouts are needed
            int scoutsNeededLeft = scoutsNeeded - uaSet.All.Count;

            // If there are too much scouts assigned to this Request
            for (; scoutsNeededLeft < 0; scoutsNeededLeft++)
            {
                var toRemove = uaSet.All.SelectMin(ua => ua.Kind.GetPurposeValue(AIName.Scouting));
                toRemove.CurrentGoal.Finish();
                toRemove.Release(this);
                uaSet.RemoveAgent(toRemove);
            }

            // Get all UnitAgents not assigned to this Request and sort them by their Suitability
            var unitAgentsSuitabilities =
                from ua in Knowledge.UnitAgents.All
                where !uaSet.All.Contains(ua)
                let p = ua.Kind.GetPurposeValue(AIName.Scouting)
                where p > 0
                let i = ua.CurrentGoalImportance
                where i < scoutsImportance
                let s = CalcSuitability(i, p)
                orderby s descending
                select new { Agent = ua, Suitability = s };

            // If there are not enough scouts assigned to this Request
            for (; scoutsNeededLeft > 0; scoutsNeededLeft--)
            {
                // if there are no more scouts to take, send request for production
                if (unitAgentsSuitabilities.Empty())
                {
                    if (!waitingForNonBusyScout)
                    {
                        SendMessage(Production, AIName.ProduceMeUnits, "1", AIName.Scout);
                        waitingForNonBusyScout = true;
                    }
                    break;
                }
                else waitingForNonBusyScout = false;
                
                // look for the mose suitable agent that can be lend for the 
                var uas = unitAgentsSuitabilities.First();
                TakeAgentNowOrLater(uas.Agent, uaSet, scoutsImportance);
            }
            
            // Get all UnitAgents assigned to this Request and sort them by their Purposes
            var requestUnitAgentsPurposes =
                from ua in uaSet.All
                let p = ua.Kind.GetPurposeValue(AIName.Scouting)
                orderby p descending
                select new { Agent = ua, Purpose = p };

            bool replaced;
            do
            {
                replaced = false;
                if (requestUnitAgentsPurposes.Empty()) return;
                if (unitAgentsSuitabilities.Empty()) return;

                var firstUAS = unitAgentsSuitabilities.First();
                var lastRUAP = requestUnitAgentsPurposes.Last();
                if (firstUAS.Suitability > lastRUAP.Purpose)
                {
                    replaced = true;

                    lastRUAP.Agent.CurrentGoal.Cancel();
                    lastRUAP.Agent.Release(this);
                    uaSet.RemoveAgent(lastRUAP.Agent);

                    TakeAgentNowOrLater(firstUAS.Agent, uaSet, scoutsImportance);
                }
            }
            while (replaced);
        }

        float CalcSuitability(float importance, float purpose)
        {
            return Mathf.Min(1 - importance, purpose);
        }

        void TakeAgentNowOrLater(UnitAgent agent, RequestUnitAgentSet uaSet, float scoutsImportance)
        {
            if (!agent.Busy)
            {
                agent.Take(this);
                agent.GiveGoal(new CoarseReconGoal(agent), scoutsImportance);
                uaSet.AddAgent(agent);
            }
            else
            {
                SendMessage(agent.Owner, AIName.HandMeOnUnit, agent.Id.ToString());
                uaSet.AddAgent(agent, true);
            }
        }

        void HandleReconUnits()
        {

        }

        class ProcessFindMeResourcesArgs
        {
            public Request request;
            public List<Request> processed;

            public ProcessFindMeResourcesArgs(Request request, List<Request> processed)
            {
                this.request = request;
                this.processed = processed;
            }
        }
    }
}