using System.Collections.Generic;
using UnityEngine;

namespace MechWars
{
    public class MapSettings : MonoBehaviour
    {
        public Spectator spectator;

        public int mapWidth = 64;
        public int mapHeight = 64;

        public float startingBuildingProgress = 0.1f;

        public List<Player> players;
        public List<GameObject> armyObjects;

        public int Width
        {
            get
            {
                if (mapWidth <= 0)
                    mapWidth = 1;
                return mapWidth;
            }
        }

        public int Height
        {
            get
            {
                if (mapHeight <= 0)
                    mapHeight = 1;
                return mapHeight;
            }
        }

        void Start()
        {
            if (spectator == null)
                throw new System.Exception("MapSettings.Spectator is NULL.");
        }
    }
}