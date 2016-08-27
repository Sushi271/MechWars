using MechWars.Utils;
using System.Collections.Generic;

namespace MechWars.AI.Regions
{
    public class BaseRegionBatch : IRegionBatch
    {
        AIBrain brain;

        public RegionBatch RegionBatch { get; private set; }

        public Region Region { get { return RegionBatch.Region; } }
        public RegionHull Hull { get { return RegionBatch.Hull; } }
        public RegionConvexHull ConvexHull { get { return RegionBatch.ConvexHull; } }

        public HashSet<BuildingInfo> Buildings { get; private set; }

        public bool RegionEmpty { get { return Region.AllTiles.Empty(); } }
        
        public bool SuspendUpdateBatch
        {
            get { return RegionBatch.SuspendUpdateBatch; }
            set { RegionBatch.SuspendUpdateBatch = value; }
        }

        public IRegionBatch ConcatenatedInto { get; set; }

        public BaseRegionBatch(AIBrain brain)
        {
            this.brain = brain;

            RegionBatch = new RegionBatch(false);
            Buildings = new HashSet<BuildingInfo>();
        }

        public void UpdateBatch()
        {
            RegionBatch.UpdateBatch();
        }
    }
}