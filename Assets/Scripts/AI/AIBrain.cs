using MechWars.AI.Agents;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MechWars.AI
{
    public class AIBrain : MonoBehaviour
    {
        public Player player;

        HashSet<Agent> agentsToAdd;
        HashSet<Agent> agents;

        public MainAgent MainAgent { get; private set; }
        
        void Start()
        {
            agentsToAdd = new HashSet<Agent>();
            agents = new HashSet<Agent>();

            MainAgent = new MainAgent(this);
        }

        void Update()
        {
            agents.UnionWith(agentsToAdd);
            agentsToAdd.Clear();
            foreach (var a in agents)
            {
                if (!a.Started) a.Start();
                a.Update();
            }
            agents.RemoveWhere(a => a.Finished);
        }

        public void AddAgent(Agent agent)
        {
            agentsToAdd.Add(agent);
        }
    }
}