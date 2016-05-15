using MechWars.Human;
using MechWars.PlayerInput.MouseStates;

namespace MechWars.PlayerInput
{
    public class InputController
    {
        public HumanPlayer Player { get; private set; }

        public PlayerKeyboard Keyboard { get; private set; }
        public PlayerMouse Mouse { get; private set; }

        public InputController(HumanPlayer player)
        {
            Player = player;

            Keyboard = new PlayerKeyboard(this);
            Mouse = new PlayerMouse(this);
        }

        public void Update()
        {
            Mouse.Update();
        }

        public MouseState ReadMouseState()
        {
            return Keyboard.ReadMouseState();
        }
    }
}
