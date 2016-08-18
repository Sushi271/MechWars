namespace MechWars.AI.Agents.Goals
{
    public abstract class UnitGoal : Goal
    {
        public UnitAgent UnitAgent { get; private set; }

        public UnitGoal(string name, UnitAgent unitAgent)
            : base(name, unitAgent)
        {
            UnitAgent = unitAgent;
        }
    }
}