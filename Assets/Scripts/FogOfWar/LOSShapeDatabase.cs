using MechWars.MapElements;
using System.Collections.Generic;

namespace MechWars.FogOfWar
{
    public class LOSShapeDatabase
    {
        Dictionary<float, Dictionary<MapElementShape, LOSShape>> shapes;

        public LOSShape this[float radius, MapElementShape mes]
        {
            get
            {
                Dictionary<MapElementShape, LOSShape> innerDict = null;
                var success = shapes.TryGetValue(radius, out innerDict);
                if (!success)
                {
                    innerDict = new Dictionary<MapElementShape, LOSShape>();
                    shapes[radius] = innerDict;
                }

                LOSShape shape = null;
                success = innerDict.TryGetValue(mes, out shape);
                if (!success)
                {
                    shape = new LOSShape(radius, mes);
                    innerDict[mes] = shape;
                }
                return shape;
            }
        }

        public LOSShapeDatabase()
        {
            shapes = new Dictionary<float, Dictionary<MapElementShape, LOSShape>>();
        }
        
        public void Clear()
        {
            shapes.Clear();
        }
    }
}
