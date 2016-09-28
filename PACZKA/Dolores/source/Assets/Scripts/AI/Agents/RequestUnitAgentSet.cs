using MechWars.AI.Agents.Goals;
using System.Collections.Generic;

namespace MechWars.AI.Agents
{
    public class RequestUnitAgentSet
    {
        public Agent Owner { get; private set; }
        public Request Request { get; private set; }

        public HashSet<UnitAgent> Ready { get; private set; }
        public HashSet<UnitAgent> Awaiting { get; private set; }
        public HashSet<UnitAgent> All { get; private set; }

        public RequestUnitAgentSet(Agent owner, Request request)
        {
            Owner = owner;
            Request = request;

            Ready = new HashSet<UnitAgent>();
            Awaiting = new HashSet<UnitAgent>();
            All = new HashSet<UnitAgent>();
        }

        public void AddAgent(UnitAgent agent, bool awaiting = false)
        {
            if (All.Contains(agent)) return;

            if (awaiting) Awaiting.Add(agent);
            else Ready.Add(agent);
            All.Add(agent);
        }

        public void RemoveAgent(UnitAgent agent)
        {
            if (!All.Contains(agent)) return;

            All.Remove(agent);
            Awaiting.Remove(agent);
            Ready.Remove(agent);
        }

        public void Clear()
        {
            var allCopy = new HashSet<UnitAgent>(All);
            foreach (var ua in allCopy)
                RemoveAgent(ua);
        }

        public void ReadyAgents(System.Func<UnitAgent, Goal> goalCtor, float importance)
        {
            var agentsToReady = new List<UnitAgent>(Awaiting.Count);
            foreach (var agent in Awaiting)
            {
                if (agent.Owner == Owner)
                {
                    agent.GiveGoal(goalCtor(agent), importance);
                    agentsToReady.Add(agent);
                }
            }

            Awaiting.ExceptWith(agentsToReady);
            Ready.UnionWith(agentsToReady);
        }
    }
}