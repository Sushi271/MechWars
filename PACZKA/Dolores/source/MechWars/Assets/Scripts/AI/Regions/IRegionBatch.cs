namespace MechWars.AI.Regions
{
    public interface IRegionBatch
    {
        Region Region { get; }
        RegionHull Hull { get; }
        RegionConvexHull ConvexHull { get; }

        bool SuspendUpdateBatch { get; set; }

        IRegionBatch ConcatenatedInto { get; set; }

        void UpdateBatch();
    }
}