using MechWars.PlayerInput.MouseStates;
using UnityEngine;

namespace MechWars.PlayerInput
{
    public class PlayerKeyboard
    {
        InputController inputController;

        public PlayerKeyboard(InputController inputController)
        {
            this.inputController = inputController;
        }

        public MouseState ReadMouseState()
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                return AttackMouseState.Instance;
            else if (Input.GetKeyDown(KeyCode.LeftAlt))
                return EscortMouseState.Instance;
            else if (Input.GetKeyDown(KeyCode.RightAlt))
                return LookAtMouseState.Instance;
            else if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                return ToggleSelectMouseState.Instance;
            else return DefaultMouseState.Instance;
        }
    }
}
