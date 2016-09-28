using MechWars.MapElements;
using System.Collections.Generic;

namespace MechWars.FogOfWar
{
    public class LOSShapeDatabase
    {
        Dictionary<float, Dictionary<MapElementShape, MapElementSurroundingShape>> shapes;

        public MapElementSurroundingShape this[float radius, MapElementShape mes]
        {
            get
            {
                Dictionary<MapElementShape, MapElementSurroundingShape> innerDict = null;
                var success = shapes.TryGetValue(radius, out innerDict);
                if (!success)
                {
                    innerDict = new Dictionary<MapElementShape, MapElementSurroundingShape>();
                    shapes[radius] = innerDict;
                }

                MapElementSurroundingShape shape = null;
                success = innerDict.TryGetValue(mes, out shape);
                if (!success)
                {
                    shape = new MapElementSurroundingShape(radius, mes);
                    innerDict[mes] = shape;
                }
                return shape;
            }
        }

        public LOSShapeDatabase()
        {
            shapes = new Dictionary<float, Dictionary<MapElementShape, MapElementSurroundingShape>>();
        }
        
        public void Clear()
        {
            shapes.Clear();
        }
    }
}
