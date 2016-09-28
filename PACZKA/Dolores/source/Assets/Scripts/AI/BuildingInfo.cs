using MechWars.AI.Regions;
using MechWars.MapElements;
using MechWars.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace MechWars.AI
{
    public class BuildingInfo
    {
        FilteringMapProxy mapProxy;

        public Vector2 Location { get; private set; }
        public List<IVector2> AllCoords { get; private set; }
        public Building Building
        {
            get
            {
                if (AllCoords.Count == 0)
                    throw new System.Exception("AllCoords cannot be empty.");
                Building b = null;
                foreach (var c in AllCoords)
                {
                    if (b == null)
                        b = mapProxy[c] as Building;
                    else if (b != mapProxy[c])
                        throw new System.Exception(string.Format(
                            "Two or more different Buildings in AllCoords ({0} and {1}).",
                            b, mapProxy[c]));
                }
                return b;
            }
        }
        public BaseRegionBatch RegionBatch { get; set; }

        public BuildingInfo(FilteringMapProxy mapProxy, Building building)
        {
            this.mapProxy = mapProxy;
            Location = building.Coords;
            AllCoords = new List<IVector2>(building.AllCoords);
        }
    }
}