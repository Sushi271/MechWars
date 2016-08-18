using MechWars.MapElements;
using System.Collections.Generic;
using System.Linq;

namespace MechWars.AI.Agents
{
    public class ResourceCollectorAgent : Agent
    {
        public List<Unit> Harvesters { get; private set; }

        public ResourceCollectorAgent(AIBrain brain, MainAgent parent)
            : base("ResourceCollector", brain, parent)
        {
            Harvesters = new List<Unit>();
        }

        protected override void OnStart()
        {
            Harvesters.AddRange(Army.Units.Where(u => u.mapElementName == MapElementName.Harvester));
            SendMessage(MainAgent.Recon, MessageName.FindMeResources, "0");
        }

        protected override void OnUpdate()
        {
            if (Army == null)
            {
                Finish();
                return;
            }

            Harvesters.RemoveAll(h => h.Dying);
        }
    }
}