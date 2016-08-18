namespace MechWars.AI.Agents.Goals
{
    public abstract class UnitExploreGoal : UnitGoal
    {
        public UnitExploreGoal(UnitAgent unitAgent)
            : base("UnitExplore", unitAgent)
        {
        }
    }
}