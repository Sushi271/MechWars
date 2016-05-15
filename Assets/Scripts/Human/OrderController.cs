using MechWars.MapElements;
using MechWars.MapElements.Orders;
using MechWars.MapElements.Production;
using MechWars.Utils;
using System.Linq;
using UnityEngine;

namespace MechWars.Human
{
    public class OrderController
    {
        HumanPlayer player;

        BuildingConstructionOption constOpt;
        Building constructingBuilding;

        GameObject buildShadow;

        public OrderController(HumanPlayer player)
        {
            this.player = player;
        }

        public void Update()
        {
            switch (player.MouseMode)
            {
                case MouseMode.Default:
                    OnDefaultMouseMode();
                    break;
                case MouseMode.BuildingLocation:
                    OnBuildingLocationMouseMode();
                    break;
            }
        }

        void OnDefaultMouseMode()
        {
            bool rightMouseDown = Input.GetMouseButtonDown(1);
            if (!rightMouseDown) return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool mapElementHit = Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask(Layer.MapElements));
            bool terrainHit = !mapElementHit &&
                Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask(Layer.Terrain));

            bool attackMode = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            bool escortMode = !attackMode && Input.GetKey(KeyCode.LeftAlt);

            var selected = new MapElementGroup(player.SelectionController.SelectedMapElements)
                .Where(me => me.army == player.Army);

            MapElement mapElement = null;
            IVector2? dest = null;
            if (mapElementHit)
            {
                var go = hit.collider.gameObject;
                mapElement = go.GetComponentInParent<MapElement>();
                if (mapElement == null)
                    throw new System.Exception("Non-MapElement GameObject is in MapElements layer.");
                dest = mapElement.Coords.Round();
            }
            else if (terrainHit)
            {
                dest = new Vector2(hit.point.x, hit.point.z).Round();
                mapElement = Globals.FieldReservationMap[dest.Value];
            }

            if (attackMode)
            {
                var attackerUnits = selected.Units.Where(u => u.CanAttack);
                if (mapElement != null && mapElement.CanBeAttacked)
                    attackerUnits.GiveOrder(u => new FollowAttackOrder(u, mapElement.AsEnumerable()));
                else if (dest.HasValue)
                    attackerUnits.GiveOrder(u => new AttackMoveOrder(u, dest.Value));

                var attackerBuildings = selected.Buildings.Where(b => b.CanAttack);
                if (mapElement != null && mapElement.CanBeAttacked)
                    attackerBuildings.GiveOrder(b => new StandAttackOrder(b, mapElement.AsEnumerable()));
            }
            else if (escortMode)
            {
                var unit = mapElement as Unit;
                if (unit != null && unit.army == player.Army)
                    selected.Units.GiveOrder(u => new EscortOrder(u, unit.AsEnumerable()));
                else if (dest.HasValue)
                    selected.Units.GiveOrder(u => new MoveOrder(u, dest.Value));
            }
            else
            {
                if (mapElement != null &&
                    mapElement.army != null &&
                    mapElement.army != player.Army &&
                    mapElement.CanBeAttacked)
                    selected.Units.Where(u => u.CanAttack).GiveOrder(u => new FollowAttackOrder(u, mapElement.AsEnumerable()));
                else if (mapElement != null && mapElement is Resource)
                {
                    selected.Units.Where(u => u.canCollect).GiveOrder(u => new HarvestOrder(u, (Resource)mapElement));
                    selected.Units.Where(u => !u.canCollect).GiveOrder(u => new MoveOrder(u, dest.Value));
                }
                else if (mapElement != null && mapElement is Building && (mapElement as Building).isResourceDeposit)
                {
                    selected.Units.Where(u => u.canCollect).GiveOrder(u => new HarvestOrder(u, (Building)mapElement));
                    selected.Units.Where(u => !u.canCollect).GiveOrder(u => new MoveOrder(u, dest.Value));
                }
                else if (dest != null)
                    selected.Units.GiveOrder(u => new MoveOrder(u, dest.Value));
            }
        }

        void OnBuildingLocationMouseMode()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || !constructingBuilding.Alive)
            {
                QuitBuildingLocationMode();
                return;
            }

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool terrainHit = Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask(Layer.Terrain));
            if (!terrainHit)
                return;

            var p = new Vector2(hit.point.x, hit.point.z);
            var shape = constOpt.building.Shape;
            var W = shape.Width;
            var H = shape.Height;
            //punkty do snapowania siem
            float xSnap1, xSnap2;
            if (W % 2 == 0)
            {
                xSnap1 = Mathf.Floor(p.x - 0.5f) + 0.5f;
                xSnap2 = Mathf.Ceil(p.x - 0.5f) + 0.5f;
            }
            else
            {
                xSnap1 = Mathf.Floor(p.x);
                xSnap2 = Mathf.Ceil(p.x);
            }
            float ySnap1, ySnap2;
            if (H % 2 == 0)
            {
                ySnap1 = Mathf.Floor(p.y - 0.5f) + 0.5f;
                ySnap2 = Mathf.Ceil(p.y - 0.5f) + 0.5f;
            }
            else
            {
                ySnap1 = Mathf.Floor(p.y);
                ySnap2 = Mathf.Ceil(p.y);
            }

            float x, y;
            if (Mathf.Abs(p.x - xSnap1) > Mathf.Abs(p.x - xSnap2))
                x = xSnap2;
            else
                x = xSnap1;
            if (Mathf.Abs(p.y - ySnap1) > Mathf.Abs(p.y - ySnap2))
                y = ySnap2;
            else
                y = ySnap1;

            p = new Vector2(x, y);

            constOpt.building.Coords = p; //ustawienie prefabowi snapowane współrzędne
            var allCoords = constOpt.building.AllCoords.ToList();
            bool isOccu = false;
            foreach (var c in allCoords) //dla każdego c we współrzędnych, które zajmie budynek
            {
                if (Globals.FieldReservationMap[c] != null) // jeżeli jedno z pól jest zajete
                {
                    isOccu = true;
                    break;
                }
            }

            buildShadow.transform.position = new Vector3(p.x, 0, p.y);


            if (Input.GetMouseButtonDown(0))
            {
                if (isOccu)
                    Debug.Log(string.Format("Cannot place building {0} in location {1} - it's occupied.", constOpt.building, p));
                else if (CheckResources(constOpt))
                {
                    var buildingToConst = constructingBuilding.Construct(constOpt, p);
                    constructingBuilding.GiveOrder(new BuildingConstructionOrder(constructingBuilding, buildingToConst));
                    QuitBuildingLocationMode();
                }
            }
        }

        void QuitBuildingLocationMode()
        {
            player.MouseMode = MouseMode.Default;
            Object.Destroy(buildShadow);
            buildShadow = null;
        }

        bool CheckResources(BuildingConstructionOption constOpt)
        {
            bool result = constOpt.StartCost <= player.Army.resources;
            if (!result)
                Debug.Log(string.Format("Not enough resources to start construction of building {0}.", constOpt.building));
            return result;
        }

        public void ProductionOrdered(Building orderingBuilding, UnitProductionOption productionOption)
        {
            orderingBuilding.GiveOrder(new UnitProductionOrder(orderingBuilding, productionOption.unit));
        }

        public void ConstructionOrdered(Building orderingBuilding, BuildingConstructionOption constructionOption)
        {
            if (CheckResources(constructionOption))
            {
                constructingBuilding = orderingBuilding;
                constOpt = constructionOption;
                player.MouseMode = MouseMode.BuildingLocation;
                buildShadow = Object.Instantiate(constructionOption.building.gameObject);
                buildShadow.name = constructionOption.building.gameObject.name + " shadow";
                var b = buildShadow.GetComponent<Building>();
                b.isShadow = true;
                var rs = buildShadow.GetComponentsInChildren<Renderer>();
                foreach (var r in rs)
                {
                    var m = r.material;
                    var col = m.color;
                    col.a = 0.5f;
                    m.color = col;
                }
            }
        }

        public void DevelopmentOrdered(Building orderingBuilding, TechnologyDevelopmentOption developmentOption)
        {
            orderingBuilding.GiveOrder(new TechnologyDevelopmentOrder(orderingBuilding, developmentOption.technology));
        }

        public void CancelOrder(Building building)
        {
            building.CancelCurrentOrder();
        }
    }
}