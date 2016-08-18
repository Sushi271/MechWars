using MechWars.MapElements;

namespace MechWars.AI.Agents
{
    public class UnitAgent : Agent
    {
        public MapElementKind Kind { get; private set; }
        public Unit Unit { get; private set; }

        public UnitAgentTask Task { get; private set; }

        public UnitAgent(AIBrain brain, Agent parent, Unit unit)
            : base("Unit", brain, parent)
        {
            Unit = unit;
            Kind = Knowledge.MapElementKinds[unit.mapElementName];
        }

        protected override void OnStart()
        {
        }

        protected override void OnUpdate()
        {
        }
    }
}