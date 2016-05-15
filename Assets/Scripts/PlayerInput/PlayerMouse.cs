using UnityEngine;

namespace MechWars.PlayerInput
{
    public class PlayerMouse
    {
        public Vector2 Position { get { return Input.mousePosition; } }
        public PlayerMouseButton Left { get; private set; }
        public PlayerMouseButton Right { get; private set; }

        public PlayerMouse()
        {
            Left = new PlayerMouseButton(0);
            Right = new PlayerMouseButton(1);
        }

        public void Update()
        {
            Left.Update();
            Right.Update();
        }
    }
}