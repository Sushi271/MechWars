using UnityEngine;

namespace MechWars.PlayerInput.MouseStates
{
    public class MouseStateButton : PlayerMouseButton
    {
        PlayerMouseButton ObservedMouseButton;

        public bool WasDown { get; private set; }
        
        public MouseStateButton(InputController inputController, PlayerMouseButton observedMouseButton)
            : base(inputController, observedMouseButton.Index)
        {
            ObservedMouseButton = observedMouseButton;
        }
        
        public override void Update()
        {
            if (!InputController.CarriesOrderAction && !InputController.Mouse.MapRaycast.GUIHit)
            {
                IsDown = ObservedMouseButton.IsDown;

                if (IsDown && !WasDown) WasDown = true;
            }
            if (WasDown)
            {
                IsPressed = ObservedMouseButton.IsPressed;
                IsUp = ObservedMouseButton.IsUp;

                if (!IsPressed && !IsUp) WasDown = false;
            }
        }
    }
}