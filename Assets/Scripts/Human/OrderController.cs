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
            if (!Physics.Raycast(ray, out hit, LayerMask.GetMask(Layer.Terrain)))
                return;

            if (Input.GetMouseButtonDown(1))
            {
                var dest = new Vector2(hit.point.x, hit.point.z).Round();

                var thisPlayersUnits =
                    from a in player.SelectionController.SelectedMapElements
                    let u = a as Unit
                    where u != null && u.army != null && u.army.Player == player
                    select u;

                foreach (var u in thisPlayersUnits)
                {
                    u.GiveOrder(new Move(u, dest));
                }
            }
        }
    }
}