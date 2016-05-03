using UnityEngine;

namespace MechWars.MapElements.Production
{
    public class BuildingConstructionOption : ProductionOption
    {
        public Building building;

        public int StartCost { get { return Mathf.CeilToInt(cost * Globals.Instance.startingBuildingProgress); } }
    }
}