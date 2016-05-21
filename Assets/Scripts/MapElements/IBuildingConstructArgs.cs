namespace MechWars.MapElements
{
    public interface IBuildingConstructArgs
    {
        Building Building { get; }
        int Cost { get; }
        float ProductionTime { get; }
        int StartCost { get; }

        bool CheckRequirements(Army army);
    }
}
