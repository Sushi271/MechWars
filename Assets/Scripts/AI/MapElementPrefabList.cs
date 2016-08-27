using MechWars.MapElements;
using System.Collections.Generic;
using UnityEngine;

namespace MechWars.AI
{
    public class MapElementPrefabList : MonoBehaviour
    {
        public List<MapElement> prefabs;

        Dictionary<string, MapElement> namedPrefabs;

        public MapElement this[string mapElementName]
        {
            get { return namedPrefabs[mapElementName]; }
        }

        void Start()
        {
            namedPrefabs = new Dictionary<string, MapElement>();
            foreach (var p in prefabs)
                namedPrefabs[p.mapElementName] = p;
        }
    }
}