using MechWars.AI.Goals;
using System.Collections.Generic;

namespace MechWars.AI.Agents
{
    public abstract class Agent
    {
        public string Name { get; private set; }
        public AIBrain Brain { get; private set; }
        public Agent Parent { get; private set; }

        public bool Finished { get; private set; }

        List<Goal> goals;

        public Agent(string name, AIBrain brain, Agent parent)
        {
            Brain = brain;
            Parent = parent;
            goals = new List<Goal>();
            brain.AddAgent(this);
        }

        public abstract void Update();

        protected void Finish()
        {
            Finished = true;
        }

        public override string ToString()
        {
            return string.Format("Agent \"{0}\"", Name);
        }
    }
}