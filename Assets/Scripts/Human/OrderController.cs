using MechWars.MapElements;
using MechWars.MapElements.Orders;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.Human
{
    public class OrderController
    {
        HumanPlayer player;
        public MouseMode MouseMode { get; private set; }

        BuildingConstructionOption constOpt;
        Building constructingBuilding;

        GameObject buildShadow;

        public OrderController(HumanPlayer player)
        {
            this.player = player;
        }

        public void Update()
        {
            if (MouseMode == MouseMode.Default)
                DefaultMouseMode();
            else if (MouseMode == MouseMode.BuildingLocation)
                BuildingLocationMouseMode();

        }

        void DefaultMouseMode()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool mapElementHit = Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask(Layer.MapElements));
            bool terrainHit;
            if (mapElementHit) terrainHit = false;
            else terrainHit = Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask(Layer.Terrain));

            if (Input.GetMouseButtonDown(1))
            {
                var thisPlayersUnits =
                    from a in player.SelectionController.SelectedMapElements
                    let u = a as Unit
                    where u != null && u.army != null && u.army == player.Army
                    select u;
                MapElement mapElement = null;
                IVector2? dest = null;
                if (mapElementHit)
                {
                    var go = hit.collider.gameObject;
                    mapElement = go.GetComponentInParent<MapElement>();
                    if (mapElement == null)
                        throw new System.Exception("Non-MapElement GameObject is in MapElements layer.");
                }
                else if (terrainHit)
                {
                    dest = new Vector2(hit.point.x, hit.point.z).Round();
                    mapElement = Globals.FieldReservationMap[dest.Value];
                }
                if (mapElement != null)
                {
                    if (mapElement.Interactible)
                    {
                        if (mapElement is Resource)
                            foreach (var u in thisPlayersUnits)
                            {
                                if (u.canCollectResources)
                                    u.GiveOrder(new HarvestOrder(u, (Resource)mapElement));
                            }
                        else if (mapElement.army == player.Army)
                            foreach (var u in thisPlayersUnits)
                                u.GiveOrder(new EscortOrder(u, mapElement));
                        else
                            foreach (var u in thisPlayersUnits)
                                if (u.canAttack)
                                    u.GiveOrder(new FollowAttackOrder(u, mapElement));
                    }
                }
                else if (dest != null)
                {
                    foreach (var u in thisPlayersUnits)
                        u.GiveOrder(new MoveOrder(u, dest.Value));
                }
            }

            var thisPlayersBuildings =
                from a in player.SelectionController.SelectedMapElements
                let b = a as Building
                where b != null && b.army != null && b.army == player.Army
                select b;
            constructingBuilding = thisPlayersBuildings.FirstOrDefault();
            if (constructingBuilding != null)
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    if (!constructingBuilding.UnderConstruction)
                        constructingBuilding.CancelCurrentOrder();
                }

                if (Input.GetKeyDown(KeyCode.P))
                {
                    if (!constructingBuilding.UnderConstruction)
                    {
                        var prodOpt = constructingBuilding.TEMP_selectedProductionOption;
                        if (prodOpt != null)
                        {
                            var unit = prodOpt.unit;
                            constructingBuilding.GiveOrder(new UnitProductionOrder(constructingBuilding, unit));
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.RightBracket))
                    constructingBuilding.TEMP_NextProductionOption();
                if (Input.GetKeyDown(KeyCode.LeftBracket))
                    constructingBuilding.TEMP_PreviousProductionOption();

                if (Input.GetKeyDown(KeyCode.L))
                {
                    if (!constructingBuilding.UnderConstruction)
                    {
                        constOpt = constructingBuilding.TEMP_selectedConstructionOption;
                        if (constOpt != null)
                        {
                            MouseMode = MouseMode.BuildingLocation;
                            buildShadow = GameObject.Instantiate(constOpt.building.gameObject);
                            buildShadow.name = constOpt.building.gameObject.name + " shadow";
                            var b = buildShadow.GetComponent<Building>();
                            b.generateResourcesOnDeath = false;
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
                }
                if (Input.GetKeyDown(KeyCode.Semicolon))
                    constructingBuilding.TEMP_NextConstructionOption();
                if (Input.GetKeyDown(KeyCode.Quote))
                    constructingBuilding.TEMP_PreviousConstructionOption();
            }
        }

        void BuildingLocationMouseMode()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelBuild();
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
                if (!Globals.FieldReservationMap.CoordsInside(c) || // jeżeli jedno z pól jest poza mapą
                    Globals.FieldReservationMap[c] != null) // lub jeżeli jedno z pól jest zajete
                {
                    isOccu = true;
                    break;
                }
            }

            buildShadow.transform.position = new Vector3(p.x, 0, p.y);


            if (Input.GetMouseButtonDown(0))
            {
                if (isOccu)
                    Debug.Log(string.Format("Can't place building {0} in location {1} - it's occupied.", constOpt.building, p));
                else
                {
                    var buildingToConst = constructingBuilding.Construct(constOpt, p);
                    constructingBuilding.GiveOrder(new ConstructionOrder(constructingBuilding, buildingToConst));
                    CancelBuild();
                }
            }
        }

        void CancelBuild()
        {
            MouseMode = MouseMode.Default;
            GameObject.Destroy(buildShadow);
            buildShadow = null;
        }
    }
}