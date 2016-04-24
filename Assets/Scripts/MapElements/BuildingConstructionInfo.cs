using UnityEngine;

namespace MechWars.MapElements
{
    public class BuildingConstructionInfo
    {
        public Building Building { get; private set; }
        public int Cost { get; private set; }
        public float ConstructionTime { get; private set; }

        public float TotalProgress { get; set; }
        public int Paid { get; set; }

        public BuildingConstructionInfo(BuildingConstructionOption constructionOption)
        {
            Building = constructionOption.building;
            Cost = constructionOption.cost;
            ConstructionTime = constructionOption.constructionTime;

            TotalProgress = 0.1f;
        }
    }
}