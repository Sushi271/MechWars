namespace MechWars.AI.Agents.Goals
{
    public abstract class ExploreGoal : UnitAgentGoal
    {
        public ExploreGoal(UnitAgent unitAgent)
            : base("Explore", unitAgent)
        {
        }
    }
}