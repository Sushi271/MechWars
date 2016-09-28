using System;

namespace MechWars.AI.Agents
{
    public class MainAgent : Agent
    {
        public new KnowledgeAgent Knowledge { get; private set; }
        public new ReconAgent Recon { get; private set; }
        public new ConstructionAgent Construction { get; private set; }
        public new ProductionAgent Production { get; private set; }
        public new ResourceCollectorAgent ResourceCollector { get; private set; }

        public MainAgent(AIBrain brain)
            : base("Main", brain, null)
        {
        }

        protected override void OnStart()
        {
            Knowledge = new KnowledgeAgent(Brain, this);
            Recon = new ReconAgent(Brain, this);
            Construction = new ConstructionAgent(Brain, this);
            Production = new ProductionAgent(Brain, this);
            ResourceCollector = new ResourceCollectorAgent(Brain, this);
        }
    }
}