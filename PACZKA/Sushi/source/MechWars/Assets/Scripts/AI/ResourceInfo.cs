using MechWars.AI.Regions;
using MechWars.MapElements;
using MechWars.Utils;

namespace MechWars.AI
{
    public class ResourceInfo
    {
        FilteringMapProxy mapProxy;

        public IVector2 Location { get; private set; }
        public Resource Resource { get { return mapProxy[Location] as Resource; } }
        public ResourceRegionBatch RegionBatch { get; set; }

        public ResourceInfo(FilteringMapProxy mapProxy, IVector2 location)
        {
            this.mapProxy = mapProxy;
            Location = location;
        }
    }
}