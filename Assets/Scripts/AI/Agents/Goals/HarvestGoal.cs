namespace MechWars.AI.Agents.Goals
{
    public abstract class HarvestGoal : UnitAgentGoal
    {
        public HarvestGoal(UnitAgent unitAgent)
            : base("Harvest", unitAgent)
        {
        }
    }
}