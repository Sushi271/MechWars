using MechWars.MapElements;
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

        public MapElementKindDictionary(AIBrain brain)
        {
            dict = new Dictionary<string, MapElementKind>();

            InitializeDictionary(brain);
        }

        void InitializeDictionary(AIBrain brain)
        {
            Add(new MapElementKind(brain.MapElementPrefabList[AIName.Scout],
                MapElementShape.DefaultShape, MapElementPurpose.Scouting(1f)));
            Add(new MapElementKind(brain.MapElementPrefabList[AIName.Harvester],
                MapElementShape.DefaultShape, MapElementPurpose.Scouting(0.2f), MapElementPurpose.Harvesting(0.8f)));
            Add(new MapElementKind(brain.MapElementPrefabList[AIName.ConstructionYard],
                Globals.ShapeDatabase[brain.MapElementPrefabList[AIName.ConstructionYard]]));
            Add(new MapElementKind(brain.MapElementPrefabList[AIName.Refinery],
                Globals.ShapeDatabase[brain.MapElementPrefabList[AIName.Refinery]]));
            Add(new MapElementKind(brain.MapElementPrefabList[AIName.Factory],
                Globals.ShapeDatabase[brain.MapElementPrefabList[AIName.Factory]]));
        }

        void Add(MapElementKind kind)
        {
            dict.Add(kind.Name, kind);
        }
    }
}