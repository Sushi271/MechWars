using UnityEngine;

namespace MechWars.PlayerInput.MouseStates
{
    public class MouseStateButton : PlayerMouseButton
    {
        InputController inputController;

        public bool WasDown { get; private set; }

        public MouseStateButton(InputController inputController, int index)
            : base(index)
        {
            this.inputController = inputController;
        }

        public override void Update()
        {
            if (!inputController.CarriesOrderAction)
            {
                IsDown = Input.GetMouseButtonDown(Index);

                if (IsDown && !WasDown) WasDown = true;
            }
            if (WasDown)
            {
                IsPressed = Input.GetMouseButton(Index);
                IsUp = Input.GetMouseButtonUp(Index);

                if (!IsPressed && !IsUp) WasDown = false;
            }
        }
    }
}