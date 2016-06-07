using System.Collections.Generic;
using MechWars.MapElements;
using UnityEngine;

namespace MechWars
{
    [System.Serializable]
    public class Army : MonoBehaviour
    {
        public HashSet<Unit> Units { get; private set; }
        public HashSet<Building> Buildings { get; private set; }
        public TechnologyController Technologies { get; private set; }

        public event System.Action OnBuildingConstructionFinished;

        public string armyName;

        public int resources;

        public Texture hpBarMain;
        public Texture hpBarSide;
        public Texture hpBarTop;

        public Sprite unitMarker;
        public Sprite buildingMarker;

        public Army()
        {
            Units = new HashSet<Unit>();
            Buildings = new HashSet<Building>();
            Technologies = new TechnologyController(this);
        }

        public void AddMapElement(MapElement mapElement)
        {
            if (mapElement is Unit)
                Units.Add((Unit)mapElement);
            else if (mapElement is Building)
            {
                var building = (Building)mapElement;
                Buildings.Add(building);
                building.OnConstructionFinished += Building_OnConstructionFinished;
            }
            else Debug.LogWarning("MapElement to add to army must be either of type Unit or Building.");
        }

        public void RemoveMapElement(MapElement mapElement)
        {
            if (mapElement is Unit)
                Units.Remove((Unit)mapElement);
            else if (mapElement is Building)
            {
                var building = (Building)mapElement;
                Buildings.Remove(building);
                building.OnConstructionFinished -= Building_OnConstructionFinished;
            }
            else Debug.LogWarning("MapElement to remove from army must be either of type Unit or Building.");
        }

        private void Building_OnConstructionFinished()
        {
            if (OnBuildingConstructionFinished != null)
                OnBuildingConstructionFinished();
        }
    }
}