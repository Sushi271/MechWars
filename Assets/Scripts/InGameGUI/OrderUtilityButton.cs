using MechWars.MapElements;
using MechWars.MapElements.Orders.Actions;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MechWars.InGameGUI
{
    public class OrderUtilityButton : MonoBehaviour, IDescriptionProvider
    {
        public CanvasScript canvasScript;
        
        public KeyCode hotkey;

        [TextArea(3, 15)]
        public string description;
        public string Description { get { return description; } }

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
            Globals.Spectator.InputController.SelectionMonitor.ClearSelection();
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
