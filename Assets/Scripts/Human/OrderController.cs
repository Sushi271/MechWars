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
                if (mapElementHit)
                {
                    var go = hit.collider.gameObject;
                    var mapElement = go.GetComponentInParent<MapElement>();
                    if (mapElement == null)
                        throw new System.Exception("Non-MapElement GameObject is in MapElements layer.");
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
                if (terrainHit)
                {
                    var dest = new Vector2(hit.point.x, hit.point.z).Round();
                    foreach (var u in thisPlayersUnits)
                        u.GiveOrder(new MoveOrder(u, dest));
                }
            }

        }
    }
}