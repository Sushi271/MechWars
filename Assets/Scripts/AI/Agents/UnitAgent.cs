using MechWars.MapElements;

namespace MechWars.AI.Agents
{
    public class UnitAgent : Agent
    {
        static int nextId;

        public int Id { get; private set; }

        public MapElementKind Kind { get; private set; }
        public Unit Unit { get; private set; }

        public Agent Owner { get; private set; }
        public bool Busy { get { return Owner != null; } }

        public UnitAgent(AIBrain brain, Agent parent, Unit unit)
            : base("Unit", brain, parent)
        {
            Unit = unit;
            Kind = Knowledge.MapElementKinds[unit.mapElementName];
            Id = nextId++;
        }

        protected override void OnStart()
        {
        }

        protected override void OnUpdate()
        {
            if (Unit.Dying)
            {
                Knowledge.UnitAgents.Remove(this);
                Finish();
                return;
            }
        }

        public bool Take(Agent takingAgent)
        {
            if (Busy) return false;
            Owner = takingAgent;
            return true;
        }

        public void Release(Agent releasingAgent)
        {
            if (releasingAgent != Owner)
                throw new System.Exception(string.Format("Agent {0} cannot release UnitAgent - it is not the Owner.", releasingAgent));
            if (!Busy)
                throw new System.Exception("Cannot release UnitAgent - it's not Busy.");

            Owner = null;
        }

        public void HandOn(Agent releasingAgent, Agent takingAgent)
        {
            Release(releasingAgent);
            Take(takingAgent);
        }
    }
}