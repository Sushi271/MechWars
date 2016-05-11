using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.MapElements.Production
{
    public class ProductionOption : MonoBehaviour
    {
        public int cost;
        public float productionTime;

        public List<Building> buildingRequirements;
        public List<Technology> technologyRequirements;

        public bool CheckRequirements(Army army)
        {
            if (army == null)
                return false;

            var noBuildingRequirements = buildingRequirements.Count == 0;
            var buildingRequirementsMet = buildingRequirements
                .All(b => army.Buildings.Where(b2 => !b2.UnderConstruction)
                .Select(b2 => b2.mapElementName)
                .Distinct().Any(n => n == b.mapElementName));
            var noTechnologyRequirements = technologyRequirements.Count == 0;
            var technologyRequirementsMet = army != null && technologyRequirements
                .All(t => army.Technologies.DevelopedTechnologies.Any(t2 => t2.IsTheSameAs(t)));

            return
                (noBuildingRequirements || buildingRequirementsMet) &&
                (noTechnologyRequirements || technologyRequirementsMet);
        }
    }
}
