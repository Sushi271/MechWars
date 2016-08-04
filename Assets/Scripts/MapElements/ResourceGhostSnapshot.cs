using MechWars.MapElements.Statistics;

namespace MechWars.MapElements
{
    public class ResourceGhostSnapshot
    {
        public int MaxValue { get; private set; }

        public ResourceGhostSnapshot(Resource original, Resource newBuilding)
        {
            MaxValue = original.MaxValue;
        }
    }
}