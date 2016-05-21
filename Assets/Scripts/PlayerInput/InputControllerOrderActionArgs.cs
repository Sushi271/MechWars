using MechWars.MapElements;
using MechWars.MapElements.Orders.Actions;
using MechWars.Utils;
using System.Collections.Generic;

namespace MechWars.PlayerInput
{
    public class InputControllerOrderActionArgs : IOrderActionArgs
    {
        InputController inputController;

        public IVector2 Destination { get { return inputController.Mouse.MapRaycast.Coords.Value; } }
        public IEnumerable<MapElement> Targets { get { return inputController.HoverController.HoveredMapElements; } }
        public ICanCreateOrderArgs CanCreateOrderArgs { get; private set; }

        public InputControllerOrderActionArgs(InputController inputController)
        {
            this.inputController = inputController;
            CanCreateOrderArgs = new InputControllerCanCreateOrderArgs(inputController);
        }
    }
}