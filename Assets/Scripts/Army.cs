using System.Collections.Generic;
using MechWars.MapElements;
using UnityEngine;

namespace MechWars
{
    [System.Serializable]
    public class Army : MonoBehaviour
    {
        public string armyName;

        public Player Player { get; private set; }

        public List<Unit> Units { get; private set; }

        public Army()
        {
            Units = new List<Unit>();
        }

        void Start()
        {
            var mapElements = GameObject.FindGameObjectsWithTag(Tag.MapElement);
            foreach (var a in mapElements)
            {
                var unit = a.GetComponent<Unit>();
                if (unit != null && unit.army == this)
                    Units.Add(unit);
            }
            
            Player = Globals.GetPlayerForArmy(this);
        }
    }
}