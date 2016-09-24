using MechWars.MapElements;
using MechWars.MapElements.Orders.Actions;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MechWars.InGameGUI
{
    public class OrderActionButton : MonoBehaviour, IDescriptionProvider
    {
        public CanvasScript canvasScript;

        // akcja ktora dziala ten button
        public OrderAction orderAction;

        public KeyCode hotkey;

        [TextArea(3, 15)]
        public string description;
        public string Description { get { return description; } }

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

        public void OnPointerEnter()
        {
            // na wejscie kursora ustawiamy canvasowi podswietlony button na TEN
            canvasScript.SetHoveredButton(GetComponent<Button>());
        }

        public void OnPointerExit()
        {
            // na wyjscie kursora ustawiamy canvasowi podswietlony button na NULL (zaden)
            canvasScript.SetHoveredButton(null);
        }
    }
}
