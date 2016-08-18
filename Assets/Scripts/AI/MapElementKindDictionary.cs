using System.Collections.Generic;

namespace MechWars.AI
{
    public class MapElementKindDictionary
    {
        Dictionary<string, MapElementKind> dict;

        public MapElementKind this[string mapElementName]
        {
            get { return dict[mapElementName]; }
        }

        public MapElementKindDictionary()
        {
            dict = new Dictionary<string, MapElementKind>();

            InitializeDictionary();
        }

        void InitializeDictionary()
        {
            Add(new MapElementKind("Harvester",
                MapElementPurpose.Scouting(0.2f), MapElementPurpose.Harvesting(0.8f)));
        }

        void Add(MapElementKind kind)
        {
            dict.Add(kind.Name, kind);
        }
    }
}