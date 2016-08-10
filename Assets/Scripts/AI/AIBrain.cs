using MechWars.AI.Agents;
using System.Collections.Generic;
using UnityEngine;

namespace MechWars.AI
{
    public class AIBrain : MonoBehaviour
    {
        public Player player;
        
        HashSet<Agent> agents;
        
        void Start()
        {
            agents = new HashSet<Agent>();
            new MainAgent(this);
        }

        void Update()
        {
            foreach (var a in agents)
                a.Update();
            agents.RemoveWhere(a => a.Finished);
        }

        public void AddAgent(Agent agent)
        {
            agents.Add(agent);
        }
    }
}