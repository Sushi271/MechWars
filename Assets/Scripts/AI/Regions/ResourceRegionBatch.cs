using MechWars.MapElements;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;

namespace MechWars.AI.Regions
{
    public class ResourceRegionBatch : IRegionBatch
    {
        AIBrain brain;

        public RegionBatch RegionBatch { get; private set; }

        public Region Region { get { return RegionBatch.Region; } }
        public RegionHull Hull { get { return RegionBatch.Hull; } }
        public RegionConvexHull ConvexHull { get { return RegionBatch.ConvexHull; } }

        public HashSet<ResourceInfo> Resources { get; private set; }

        public bool RegionEmpty { get { return Region.AllTiles.Empty(); } }

        public int TotalResourceValue
        {
            get
            {
                return Region.AllTiles.Sum(t =>
                {
                    var res = brain.MapProxy[t] as Resource;
                    if (res == null) return 0;
                    return res.value;
                });
            }
        }

        public bool SuspendUpdateBatch
        {
            get { return RegionBatch.SuspendUpdateBatch; }
            set { RegionBatch.SuspendUpdateBatch = value; }
        }

        public IRegionBatch ConcatenatedInto { get; set; }

        public ResourceRegionBatch(AIBrain brain, bool suspendUpdateBatch = false)
        {
            this.brain = brain;

            RegionBatch = new RegionBatch(suspendUpdateBatch);
            Resources = new HashSet<ResourceInfo>();
        }

        public void UpdateBatch()
        {
            RegionBatch.UpdateBatch();
        }

        public bool HasResource(Resource resource)
        {
            return Resources.Any(ri => ri.Resource == resource);
        }
    }
}