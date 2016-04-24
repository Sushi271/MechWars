using MechWars.MapElements;
using MechWars.MapElements.Orders;
using MechWars.Utils;
using System.Linq;
using UnityEngine;

namespace MechWars.Human
{
    public class OrderController
    {
        HumanPlayer player;

        public OrderController(HumanPlayer player)
        {
            this.player = player;
        }

        public void Update()
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
            var building = thisPlayersBuildings.FirstOrDefault();
            if (building != null)
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    if (!building.UnderConstruction)
                        building.CancelCurrentOrder();
                }

                if (Input.GetKeyDown(KeyCode.P))
                {
                    if (!building.UnderConstruction)
                    {
                        var prodOpt = building.TEMP_selectedProductionOption;
                        if (prodOpt != null)
                        {
                            var unit = prodOpt.unit;
                            building.GiveOrder(new UnitProductionOrder(building, unit));
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.RightBracket))
                    building.TEMP_NextProductionOption();
                if (Input.GetKeyDown(KeyCode.LeftBracket))
                    building.TEMP_PreviousProductionOption();

                if (Input.GetKeyDown(KeyCode.L))
                {
                    if (!building.UnderConstruction)
                    {
                        var constOpt = building.TEMP_selectedConstructionOption;
                        if (constOpt != null)
                        {
                            var buildingToConst = building.Construct(constOpt, building.TEMP_buildLocation);
                            building.GiveOrder(new ConstructionOrder(building, buildingToConst));
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.Semicolon))
                    building.TEMP_NextConstructionOption();
                if (Input.GetKeyDown(KeyCode.Quote))
                    building.TEMP_PreviousConstructionOption();
            }
        }
    }
}