using MechWars.MapElements;
using System.Collections.Generic;
using System.Linq;

namespace MechWars
{
    public class MapElementsDatabase
    {
        HashSet<MapElement> mapElements;
        HashSet<Unit> units;
        HashSet<Building> buildings;
        HashSet<Resource> resources;

        public IEnumerable<MapElement> MapElements { get { return mapElements; } }
        public IEnumerable<Unit> Units { get { return units; } }
        public IEnumerable<Building> Buildings { get { return buildings; } }
        public IEnumerable<Resource> Resources { get { return resources; } }

        public IEnumerable<MapElement> All
        {
            get
            {
                return mapElements
                    .Concat(units.Cast<MapElement>())
                    .Concat(buildings.Cast<MapElement>())
                    .Concat(resources.Cast<MapElement>());
            }
        }

        public MapElementsDatabase()
        {
            mapElements = new HashSet<MapElement>();
            units = new HashSet<Unit>();
            buildings = new HashSet<Building>();
            resources = new HashSet<Resource>();
        }

        public void Insert(MapElement mapElement)
        {
            if (mapElement is Unit)
                units.Add((Unit)mapElement);
            else if (mapElement is Building)
                buildings.Add((Building)mapElement);
            else if (mapElement is Resource)
                resources.Add((Resource)mapElement);
            else mapElements.Add(mapElement);
        }
        
        public void Delete(MapElement mapElement)
        {
            if (mapElement is Unit)
                units.Remove((Unit)mapElement);
            else if (mapElement is Building)
                buildings.Remove((Building)mapElement);
            else if (mapElement is Resource)
                resources.Remove((Resource)mapElement);
            else mapElements.Remove(mapElement);
        }

        public void Clear()
        {
            mapElements.Clear();
            units.Clear();
            buildings.Clear();
            resources.Clear();
        }
    }
}
