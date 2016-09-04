using MechWars.AI.Agents.Goals;
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

        public ReconAgent(AIBrain brain, MainAgent parent)
            : base("Recon", brain, parent)
        {
            requests = new List<Request>();
            ReconUnits = new Dictionary<Request, RequestUnitAgentSet>();
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

        bool waitingForScout;
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
                if (!waitingForScout)
                {
                    SendMessage(Production, AIName.ProduceMeUnits, "1", AIName.Scout);
                    waitingForScout = true;
                }
                return;
            }
            else waitingForScout = false;

            // Determine how much scouts are needed and how important is their task
            int scoutsNeeded = scoutsNeededByPriority[r.Priority];
            float scoutsImportance = scoutsImportanceByPriority[r.Priority];

            // Get UnitAgents currently assigned to this Request and update them
            var uaSet = ReconUnits[r];
            uaSet.ReadyAgents(ua => new ReconGoal(ua), scoutsImportance);
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
                let i = ua.CurrentGoal.Importance
                let a = CalcSuitability(i, p)
                orderby a descending
                select new { Agent = ua, Suitability = a };

            // If there are not enough scouts assigned to this Request
            for (; scoutsNeededLeft > 0; scoutsNeededLeft--)
            {
                // if there are no more scouts to take, send request for production
                if (unitAgentsSuitabilities.Empty())
                {
                    if (!waitingForScout)
                    {
                        SendMessage(Production, AIName.ProduceMeUnits, "1", AIName.Scout);
                        waitingForScout = true;
                    }
                    break;
                }
                else waitingForScout = false;

                // this will take always the most suitable available non-taken UAS.
                var uas = unitAgentsSuitabilities.First();
                if (uas.Suitability >= 1 - scoutsImportance)
                {
                    var agent = uas.Agent;
                    if (!agent.Busy)
                    {
                        agent.Take(this);
                        agent.GiveGoal(new ReconGoal(agent), scoutsImportance);
                        uaSet.AddAgent(agent);
                    }
                    else
                    {
                        uaSet.AddAgent(agent, true);
                        SendMessage(agent.Owner, AIName.HandMeOnUnit, agent.Id.ToString());
                    }
                }
            }

            if (waitingForScout)
            {

            }

            // give them a new task - scout for resources
            // every update update their scouting destination basing on generated grid
            // grid must be generated from regions of unknown territory
            // every update monitor the importance of this task, or consult it with ResCol
            // release UAs once ResCol tells it's happy, or once better suited agents joined the army
        }

        float CalcSuitability(float importance, float purpose)
        {
            return Mathf.Min(1 - importance, purpose);
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