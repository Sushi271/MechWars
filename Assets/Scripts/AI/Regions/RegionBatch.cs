namespace MechWars.AI.Regions
{
    public class RegionBatch : IRegionBatch
    {
        public Region Region { get; private set; }
        public RegionHull Hull { get; private set; }
        public RegionConvexHull ConvexHull { get; private set; }

        public bool SuspendUpdateBatch { get; set; }

        public RegionBatch(bool suspendUpdateBatch = false)
        {
            Region = new Region();
            Region.RegionChanged += Region_RegionChanged;

            SuspendUpdateBatch = suspendUpdateBatch;
        }

        public void UpdateBatch()
        {
            Hull = new RegionHull(Region);
            ConvexHull = new RegionConvexHull(Hull);
        }

        private void Region_RegionChanged()
        {
            if (SuspendUpdateBatch) return;
            UpdateBatch();
        }
    }
}