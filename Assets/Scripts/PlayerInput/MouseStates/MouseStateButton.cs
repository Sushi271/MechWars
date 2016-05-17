using UnityEngine;

namespace MechWars.PlayerInput.MouseStates
{
    public class MouseStateButton : PlayerMouseButton
    {
        public bool WasDown { get; private set; }

        public MouseStateButton(InputController inputController, int index)
            : base(inputController, index)
        {
        }
        
        public override void Update()
        {
            if (!InputController.CarriesOrderAction && !InputController.Mouse.MapRaycast.GUIHit)
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