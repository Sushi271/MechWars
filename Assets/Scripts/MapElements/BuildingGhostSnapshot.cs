using MechWars.MapElements.Statistics;

namespace MechWars.MapElements
{
    public class BuildingGhostSnapshot
    {
        public BuildingConstructionInfo ConstructionInfo { get; private set; }

        public BuildingGhostSnapshot(Building original, Building newBuilding)
        {
            ConstructionInfo = original.ConstructionInfo == null ?
                null : original.ConstructionInfo.Clone(newBuilding);
        }
    }
}