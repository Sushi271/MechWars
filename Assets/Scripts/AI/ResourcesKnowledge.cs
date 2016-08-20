using MechWars.AI.Agents;
using MechWars.AI.Regions;
using MechWars.FogOfWar;
using MechWars.MapElements;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;

namespace MechWars.AI
{
    public class ResourcesKnowledge
    {
        KnowledgeAgent knowledge;

        ResourceInfo[,] resourceInfos;
        HashSet<ResourceRegionBatch> resourceRegions;

        public ResourceInfo this[int x, int y]
        {
            get { return resourceInfos[x, y]; }
            set
            {
                if (resourceInfos[x, y] == value) return;
                var oldValue = resourceInfos[x, y];
                resourceInfos[x, y] = value;
                UpdateResourceRegions(x, y, oldValue, value);
            }
        }

        public ResourceInfo this[IVector2 tile]
        {
            get { return this[tile.X, tile.Y]; }
            set { this[tile.X, tile.Y] = value; }
        }

        public IEnumerable<ResourceRegionBatch> Regions { get { return resourceRegions; } }

        public ResourcesKnowledge(KnowledgeAgent knowledge)
        {
            this.knowledge = knowledge;

            resourceInfos = new ResourceInfo[Globals.MapSettings.Size, Globals.MapSettings.Size];
            resourceRegions = new HashSet<ResourceRegionBatch>();

            Initialize();
        }

        void Initialize()
        {
            for (int x = 0; x < Globals.MapSettings.Size; x++)
                for (int y = 0; y < Globals.MapSettings.Size; y++)
                    if (knowledge.Army.VisibilityTable[x, y] != Visibility.Unknown)
                    {
                        var mapElement = knowledge.MapProxy[x, y];
                        if (mapElement is Resource)
                            this[x, y] = new ResourceInfo(knowledge.MapProxy, new IVector2(x, y));
                    }
        }

        void UpdateResourceRegions(int x, int y, ResourceInfo oldValue, ResourceInfo newValue)
        {
            var others = FindSurroundingResourceInfos(x, y, newValue);
            var regionBatches = others.SelectDistinct(ri => ri.RegionBatch);
            if (oldValue == null)
            {
                if (newValue.RegionBatch != null)
                    throw new System.Exception("Called UpdateResourceRegions with newValue, which is already in some Region.");

                ResourceRegionBatch regionBatch;
                if (regionBatches.Empty())
                {
                    regionBatch = new ResourceRegionBatch(knowledge.Brain);
                    resourceRegions.Add(regionBatch);
                }
                else regionBatch = regionBatches.First();

                newValue.RegionBatch = regionBatch;
                regionBatch.Resources.Add(newValue);
                regionBatch.Region.AddTile(x, y);

                if (regionBatches.HasAtLeast(2))
                {
                    var newRegionBatch = regionBatches.ConcatBatches(knowledge.Brain);
                    resourceRegions.ExceptWith(regionBatches);
                    resourceRegions.Add(newRegionBatch);
                }
            }
        }

        List<ResourceInfo> FindSurroundingResourceInfos(int x, int y, ResourceInfo reference)
        {
            var list = new List<ResourceInfo>();

            var shape = knowledge.Brain.ResourceRegionDetectionShape;
            for (int rx = shape.GetXMin(x), i = 0; rx <= shape.GetXMax(x); rx++, i++)
                for (int ry = shape.GetYMin(y), j = 0; ry <= shape.GetYMax(y); ry++, j++)
                    if (shape[i, j] && Globals.Map.IsInBounds(rx, ry) &&
                        resourceInfos[rx, ry] != null && resourceInfos[rx, ry] != reference)
                        list.Add(resourceInfos[rx, ry]);

            return list;
        }
    }
}