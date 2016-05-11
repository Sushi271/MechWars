using System.Collections.Generic;
using UnityEngine;

namespace MechWars.MapElements.WallNeighbourhoods
{
    public class WallNeighbourhoodDictionary : MonoBehaviour
    {
        public List<WallNeighbourhoodDefinition> wallTypes;
        public Dictionary<WallNeighbourhood, WallNeighbourhoodDefinition> WallTypesDictionary
        {
            get; private set;
        }
        public WallNeighbourhoodDictionary()
        {
            WallTypesDictionary = new Dictionary<WallNeighbourhood, WallNeighbourhoodDefinition>();
        }

        void Start()
        {
            foreach (var wt in wallTypes)
            {
                WallTypesDictionary.Add(wt.Neighbourhood, wt);
            }
        }
    }
}
