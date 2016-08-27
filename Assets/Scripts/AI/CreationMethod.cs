using System.Collections.Generic;

namespace MechWars.AI
{
    public class CreationMethod
    {
        public MapElementKind Created { get; private set; }
        public MapElementKind Creator { get; private set; }
        public int Cost { get; private set; }
        public float Time { get; private set; }

        public List<MapElementKind> BuildingRequirements { get; private set; }
        public List<TechnologyKind> TechnologyRequirements { get; private set; }

        public CreationMethod(MapElementKind created, MapElementKind creator, int cost, float time)
        {
            Created = created;
            Creator = creator;
            Cost = cost;
            Time = time;

            BuildingRequirements = new List<MapElementKind>();
            TechnologyRequirements = new List<TechnologyKind>();
        }
    }
}