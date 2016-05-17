using UnityEngine;

namespace MechWars.PlayerInput
{
    public class PlayerMouseButton
    {
        protected InputController InputController { get; private set; }
        protected int Index { get; private set; }

        public bool IsDown { get; protected set; }
        public bool IsPressed { get; protected set; }
        public bool IsUp { get; protected set; }

        public PlayerMouseButton(InputController inputController, int index)
        {
            InputController = inputController;
            Index = index;
        }

        public virtual void Update()
        {
            IsDown = Input.GetMouseButtonDown(Index);
            IsPressed = Input.GetMouseButton(Index);
            IsUp = Input.GetMouseButtonUp(Index);
        }
    }
}