using MechWars.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace MechWars.Mapping
{
    public class MapSettings : MonoBehaviour
    {
        public Spectator spectator;

        public int size = 64;

        public float startingBuildingProgress = 0.1f;

        public List<Player> players;
        public List<GameObject> armyObjects;

        public int Size
        {
            get
            {
                if (size <= 0)
                    size = 1;
                return size;
            }
        }

        void Start()
        {
            if (spectator == null)
                throw new System.Exception("MapSettings.Spectator is NULL.");
            if (!size.IsPowerOfTwo())
                throw new System.Exception(string.Format(
                    "Map size must be a power of two, but it is {0}.", size));
        }
    }
}