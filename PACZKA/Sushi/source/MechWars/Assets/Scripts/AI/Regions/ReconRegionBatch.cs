using MechWars.FogOfWar;
using MechWars.MapElements;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;

namespace MechWars.AI.Regions
{
    public class ReconRegionBatch : IRegionBatch
    {
        AIBrain brain;

        public RegionBatch RegionBatch { get; private set; }

        public Region Region { get { return RegionBatch.Region; } }
        public RegionHull Hull { get { return RegionBatch.Hull; } }
        public RegionConvexHull ConvexHull { get { return RegionBatch.ConvexHull; } }

        public bool EntirelyExplored { get; private set; }
        public int UnknownTilesCount
        {
            get
            {
                if (EntirelyExplored) return 0;

                var vt = brain.MainAgent.Army.VisibilityTable;
                var count = Region.AllTiles.Where(t => vt[t.X, t.Y] == Visibility.Unknown).Count();

                if (count == 0) EntirelyExplored = true;
                return count;
            }
        }
        public int KnownTilesCount { get { return Region.Area - UnknownTilesCount; } }
        public float ExplorationPercentage { get { return (float)KnownTilesCount / Region.Area * 100; } }

        public bool SuspendUpdateBatch
        {
            get { return RegionBatch.SuspendUpdateBatch; }
            set { RegionBatch.SuspendUpdateBatch = value; }
        }

        public IRegionBatch ConcatenatedInto { get; set; }

        public ReconRegionBatch(AIBrain brain, bool suspendUpdateBatch = false)
        {
            this.brain = brain;

            RegionBatch = new RegionBatch(suspendUpdateBatch);
        }

        public void UpdateBatch()
        {
            RegionBatch.UpdateBatch();
        }
    }
}