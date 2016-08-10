using System;

namespace MechWars.AI.Agents
{
    public class MainAgent : Agent
    {
        public MainAgent(AIBrain brain)
            : base("Main", brain, null)
        {
        }

        public override void Update()
        {
        }

        public override string ToString()
        {
            return string.Format("Agent \"{0}\"", Name);
        }
    }
}