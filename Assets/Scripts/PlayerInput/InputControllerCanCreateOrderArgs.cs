using MechWars.MapElements.Orders.Actions;

namespace MechWars.PlayerInput
{
    public class InputControllerCanCreateOrderArgs : ICanCreateOrderArgs
    {
        InputController inputController;

        public IBuildingPlacement BuildingPlacement { get { return inputController.BuildingShadow; } }
        public Player Player { get { return inputController.Spectator.player; } }

        public InputControllerCanCreateOrderArgs(InputController inputController)
        {
            this.inputController = inputController;
        }
    }
}