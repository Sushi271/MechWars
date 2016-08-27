using System;

namespace MechWars.AI.Agents
{
    public class MainAgent : Agent
    {
        public new KnowledgeAgent Knowledge { get; private set; }
        public new ReconAgent Recon { get; private set; }
        public new ConstructionAgent Construction { get; private set; }
        public new ProductionAgent Production { get; private set; }

        ResourceCollectorAgent resourceCollector;

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

            resourceCollector = new ResourceCollectorAgent(Brain, this);
        }
    }
}