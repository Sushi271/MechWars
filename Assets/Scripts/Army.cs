using System.Collections.Generic;
using MechWars.MapElements;
using UnityEngine;
using MechWars.FogOfWar;
using MechWars.Mapping;

namespace MechWars
{
    [System.Serializable]
    public class Army : MonoBehaviour
    {
        public HashSet<Unit> Units { get; private set; }
        public HashSet<Building> Buildings { get; private set; }
        public TechnologyController Technologies { get; private set; }

        public VisibilityTable VisibilityTable { get; private set; }
        public QuadTree ResourcesQuadTree { get; private set; }
        public QuadTree AlliesQuadTree { get; private set; }
        public QuadTree EnemiesQuadTree { get; private set; }

        public event System.Action OnBuildingConstructionFinished;
        public event System.Action<MapElement> OnVisibleMapElementCreated;
        public event System.Action<MapElement> OnVisibleMapElementDied;

        public string armyName;

        public bool actionsVisible;

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

        void Start()
        {
            VisibilityTable = new VisibilityTable(this);
            ResourcesQuadTree = Globals.Map.CreateQuadTree();
            AlliesQuadTree = Globals.Map.CreateQuadTree();
            EnemiesQuadTree = Globals.Map.CreateQuadTree();
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
            else Debug.LogWarning("In order to add MapElement to Army, it must be either of type Unit or Building.");
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
            else Debug.LogWarning("In order to remove MapElement from Army, it must be either of type Unit or Building.");
        }

        public void InvokeOnVisibleMapElementCreated(MapElement mapElement)
        {
            if (OnVisibleMapElementCreated != null)
                OnVisibleMapElementCreated(mapElement);
        }

        public void InvokeOnVisibleMapElementDied(MapElement mapElement)
        {
            if (OnVisibleMapElementDied != null)
                OnVisibleMapElementDied(mapElement);
        }

        private void Building_OnConstructionFinished()
        {
            if (OnBuildingConstructionFinished != null)
                OnBuildingConstructionFinished();
        }
    }
}