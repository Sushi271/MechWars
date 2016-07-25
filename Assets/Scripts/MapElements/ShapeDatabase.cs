using MechWars.MapElements;
using System.Collections.Generic;

namespace MechWars
{
    public class ShapeDatabase
    {
        Dictionary<string, MapElementShape> shapes;

        public MapElementShape this[MapElement me, bool forceRead = false]
        {
            get
            {
                MapElementShape shape = null;
                if (!forceRead)
                    forceRead = !shapes.TryGetValue(me.mapElementName, out shape);
                if (forceRead)
                {
                    shape = ReadShape(me);
                    shapes[me.mapElementName] = shape;
                }
                return shape;
            }
        }

        public ShapeDatabase()
        {
            shapes = new Dictionary<string, MapElementShape>();
        }

        MapElementShape ReadShape(MapElement mapElement)
        {
            MapElementShape shape;
            if (mapElement.shapeFile == null)
                shape = MapElementShape.DefaultShape;
            else shape = MapElementShape.FromString(mapElement.shapeFile.text);
            return shape;
        }

        public void Clear()
        {
            shapes.Clear();
        }
    }
}
