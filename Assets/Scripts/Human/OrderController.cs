using System.Linq;
using MechWars.MapElements;
using MechWars.Orders;
using MechWars.Utils;
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
            bool mapElementHit = Physics.Raycast(ray, out hit, LayerMask.GetMask(Layer.MapElements));
            bool terrainHit;
            if (mapElementHit) terrainHit = false;
            terrainHit = Physics.Raycast(ray, out hit, LayerMask.GetMask(Layer.Terrain));

            if (Input.GetMouseButtonDown(1))
            {
                var thisPlayersUnits =
                    from a in player.SelectionController.SelectedMapElements
                    let u = a as Unit
                    where u != null && u.army != null && u.army == player.Army
                    select u;
                if (terrainHit)
                {
                    var dest = new Vector2(hit.point.x, hit.point.z).Round();
                    foreach (var u in thisPlayersUnits)
                        u.GiveOrder(new Move(u, dest));
                }
                else if (mapElementHit)
                {
                    var go = hit.collider.gameObject;
                    var mapElement = go.GetComponent<MapElement>();
                    if (mapElement == null)
                        throw new System.Exception("Non-MapElement GameObject is in MapElements layer.");
                    if (mapElement.Interactible)
                    {
                        if (mapElement.army == player.Army)
                            foreach (var u in thisPlayersUnits)
                                u.GiveOrder(new Escort(u, mapElement));
                        else 
                            foreach (var u in thisPlayersUnits)
                                u.GiveOrder(new Attack(u, mapElement));
                    }
                }
            }

        }
    }
}