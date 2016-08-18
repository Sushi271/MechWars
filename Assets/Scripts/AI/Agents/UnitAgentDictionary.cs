using System.Collections.Generic;

namespace MechWars.AI.Agents
{
    public class UnitAgentDictionary
    {
        Dictionary<MapElementKind, HashSet<UnitAgent>> dict;

        public HashSet<UnitAgent> this[MapElementKind mapElementKind]
        {
            get { return dict[mapElementKind]; }
        }

        public UnitAgentDictionary()
        {
            dict = new Dictionary<MapElementKind, HashSet<UnitAgent>>();
        }

        public void Add(UnitAgent unitAgent)
        {
            HashSet<UnitAgent> agents;
            if (!dict.TryGetValue(unitAgent.Kind, out agents))
            {
                agents = new HashSet<UnitAgent>();
                dict[unitAgent.Kind] = agents;
            }
            agents.Add(unitAgent);
        }

        public void Remove(UnitAgent unitAgent)
        {
            HashSet<UnitAgent> agents;
            if (!dict.TryGetValue(unitAgent.Kind, out agents) ||
                !agents.Contains(unitAgent))
                throw new System.Exception(string.Format("No UnitAgent to remove (UnitAgent {0}).", unitAgent));

            agents.Remove(unitAgent);
            if (agents.Count == 0)
                dict.Remove(unitAgent.Kind);
        }

    }
}