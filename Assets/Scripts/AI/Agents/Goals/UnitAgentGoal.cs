namespace MechWars.AI.Agents.Goals
{
    public abstract class UnitAgentGoal : Goal
    {
        public UnitAgent UnitAgent { get; private set; }

        public UnitAgentGoal(string name, UnitAgent unitAgent)
            : base(name, unitAgent)
        {
            UnitAgent = unitAgent;
        }
    }
}