using MechWars.MapElements;
using MechWars.Utils;

namespace MechWars.Mapping
{
    public class QuadTreeMapElement
    {
        public MapElement MapElement { get; private set; }
        public IVector2 Coords { get; private set; }

        public QuadTreeMapElement(MapElement mapElement, IVector2 coords)
        {
            MapElement = mapElement;
            Coords = coords;
        }
    }
}
