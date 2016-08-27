﻿using MechWars.MapElements;
using System.Collections.Generic;

namespace MechWars.AI.Agents
{
    public class UnitAgentDictionary
    {
        KnowledgeAgent knowledge;

        Dictionary<MapElementKind, HashSet<UnitAgent>> kindDict;
        Dictionary<Unit, UnitAgent> unitDict;

        public HashSet<UnitAgent> this[MapElementKind mapElementKind]
        {
            get { return kindDict[mapElementKind]; }
        }

        public HashSet<UnitAgent> this[string mapElementName]
        {
            get { return this[knowledge.MapElementKinds[mapElementName]]; }
        }

        public UnitAgent this[Unit unit]
        {
            get { return unitDict[unit]; }
        }

        public UnitAgentDictionary(KnowledgeAgent knowledge)
        {
            this.knowledge = knowledge;

            kindDict = new Dictionary<MapElementKind, HashSet<UnitAgent>>();
            unitDict = new Dictionary<Unit, UnitAgent>();
        }

        public void Add(UnitAgent unitAgent)
        {
            HashSet<UnitAgent> agents;
            if (!kindDict.TryGetValue(unitAgent.Kind, out agents))
            {
                agents = new HashSet<UnitAgent>();
                kindDict[unitAgent.Kind] = agents;
            }
            agents.Add(unitAgent);

            unitDict.Add(unitAgent.Unit, unitAgent);
        }

        public void Remove(UnitAgent unitAgent)
        {
            HashSet<UnitAgent> agents;
            if (!kindDict.TryGetValue(unitAgent.Kind, out agents) ||
                !agents.Contains(unitAgent))
                throw new System.Exception(string.Format("No UnitAgent to remove (UnitAgent {0}).", unitAgent));

            agents.Remove(unitAgent);
            if (agents.Count == 0)
                kindDict.Remove(unitAgent.Kind);

            unitDict.Remove(unitAgent.Unit);
        }

    }
}