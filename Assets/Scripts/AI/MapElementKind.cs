using MechWars.MapElements;
using System.Collections.Generic;
using System.Linq;

namespace MechWars.AI
{
    public class MapElementKind : IKind
    {
        public string Name { get; private set; }
        public MapElement MapElement { get; private set; }
        public MapElementShape Shape { get; private set; }

        public Dictionary<string, MapElementPurpose> Purposes { get; private set; }
        public List<CreationMethod> CreationMethods { get; private set; }

        public MapElementKind(MapElement mapElement, MapElementShape shape, params MapElementPurpose[] purposes)
        {
            Name = mapElement.mapElementName;
            MapElement = mapElement;
            Shape = shape;
            Purposes = new Dictionary<string, MapElementPurpose>();
            foreach (var a in purposes)
                Purposes.Add(a.Name, a);
            CreationMethods = new List<CreationMethod>();

            NormalizePurposes();
        }

        void NormalizePurposes()
        {
            var sum = Purposes.Sum(kv => kv.Value.Value);
            foreach (var a in Purposes.Values)
                a.Value /= sum;
        }
    }
}