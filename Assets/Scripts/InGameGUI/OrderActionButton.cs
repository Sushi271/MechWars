using MechWars.MapElements;
using MechWars.MapElements.Orders.Actions;
using System.Linq;
using UnityEngine;

namespace MechWars.InGameGUI
{
    public class OrderActionButton : MonoBehaviour
    {
        // akcja ktora dziala ten button
        public OrderAction orderAction;

        public void OnClick_ChangeCarriedOrderAction()
        {
            // ustawiamy akcje jako "niesiona" przez myszke
            Globals.Spectator.InputController.CarriedOrderAction = orderAction;
        }

        public void OnClick_ChangeCarriedBuildingOrderAction()
        {
            var inputController = Globals.Spectator.InputController;
            var selected = inputController.SelectionMonitor.SelectedMapElements;
            // ustawiamy akcje jako "niesiona" przez myszke i tworzymy BuildingShadow oraz ConstructionRange
            inputController.CarriedOrderAction = orderAction;
            inputController.CreateShadow((BuildingConstructionOrderAction)orderAction);
            inputController.CreateConstructionRange((Building)selected.First());
        }

        public void OnClick_InvokeOrderAction()
        {
            var inputController = Globals.Spectator.InputController;
            var selected = inputController.SelectionMonitor.SelectedMapElements;

            // dla kazdej z zaznaczonych jednostek / dla zaznaczonego budynku
            foreach (var mapElement in selected)
            {
                // wywołujemy akcję od razu
                orderAction.GiveOrder(mapElement, inputController.OrderActionArgs);
            }
        }
    }
}
