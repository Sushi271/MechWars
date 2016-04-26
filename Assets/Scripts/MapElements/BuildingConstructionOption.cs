using UnityEngine;

namespace MechWars.MapElements
{
    public class BuildingConstructionOption : MonoBehaviour
    {
        public Building building;
        public int cost;
        public float constructionTime;

        public int StartCost { get { return Mathf.CeilToInt(cost * Globals.Instance.startingBuildingProgress); } }
        
        // TODO: wymagania itepe
    }
}