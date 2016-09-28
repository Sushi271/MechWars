using System.Collections.Generic;
using System.Linq;

namespace MechWars.AI.Regions
{
    public static class RegionBatchExtensions
    {
        public static T ConcatBatches<T>(this IEnumerable<T> regionBatches, System.Func<bool, T> ctor)
            where T : IRegionBatch
        {
            var concatenated = ctor(true);
            var reg = concatenated.Region;

            foreach (var r in regionBatches.Select(rb => rb.Region))
                foreach (var tile in r.AllTiles)
                    reg.AddTile(tile);

            concatenated.SuspendUpdateBatch = false;
            concatenated.UpdateBatch();
            return concatenated;
        }

        public static ResourceRegionBatch ConcatBatches(
            this IEnumerable<ResourceRegionBatch> regionBatches, AIBrain brain)
        {
            var concatenated = regionBatches.ConcatBatches(susp => new ResourceRegionBatch(brain, susp));
            concatenated.Resources.UnionWith(regionBatches.SelectMany(rb => rb.Resources));
            foreach (var rb in regionBatches)
            {
                rb.Resources.Clear();
                rb.ConcatenatedInto = concatenated;
            }
            foreach (var r in concatenated.Resources)
                r.RegionBatch = concatenated;
            return concatenated;
        }
    }
}