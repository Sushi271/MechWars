using MechWars.MapElements.Orders.Actions;
using UnityEngine;

namespace MechWars.InGameGUI
{
    public class OrderActionButton : MonoBehaviour
    {
        // akcja ktora dziala ten button
        public OrderAction orderAction;

        public void OnClick()
        {
            // ustawiamy ja jako "niesiona" przez myszke
            Globals.Spectator.InputController.CarriedOrderAction = orderAction;
        }
    }
}
