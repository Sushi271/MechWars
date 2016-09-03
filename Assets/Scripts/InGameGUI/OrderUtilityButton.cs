using MechWars.MapElements;
using System.Linq;
using UnityEngine;

namespace MechWars.InGameGUI
{
    public class OrderUtilityButton : MonoBehaviour
    {
        // Guzik Cancel na ConstYard, Factory i Laboratory
        public void OnClick_CancelProduction()
        {
            var buildings = Globals.Spectator.InputController.SelectionMonitor.SelectedMapElements.OfType<Building>();
            var bld = buildings.FirstOrDefault();

            if (bld != null)
                bld.OrderQueue.CancelCurrent();
        }

        // Guzik Cancel staly
        public void OnClick_CancelSelection()
        {
        }
    }
}
