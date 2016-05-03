using System.Collections.Generic;
using UnityEngine;

namespace MechWars.MapElements
{
    public class Technology : MonoBehaviour
    {
        public string technologyName;
        public List<StatBonus> bonuses;
        
        public override string ToString()
        {
            return technologyName ?? "";
        }

        public bool IsTheSameAs(Technology technology)
        {
            return technologyName == technology.technologyName;
        }
    }
}