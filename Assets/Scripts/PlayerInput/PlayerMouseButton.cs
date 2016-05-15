using UnityEngine;

namespace MechWars.PlayerInput
{
    public class PlayerMouseButton
    {
        int index;

        public bool IsDown { get; private set; }
        public bool IsPressed { get; private set; }
        public bool IsUp { get; private set; }

        public PlayerMouseButton(int index)
        {
            this.index = index;
        }

        public void Update()
        {
            IsDown = Input.GetMouseButtonDown(index);
            IsPressed = Input.GetMouseButton(index);
            IsUp = Input.GetMouseButtonUp(index);
        }
    }
}