using MechWars.Human;
using MechWars.MapElements.Orders.Actions;
using MechWars.PlayerInput.MouseStates;
using UnityEngine;

namespace MechWars.PlayerInput
{
    public class PlayerMouse
    {
        InputController inputController;
        HoverController hoverController;

        public HumanPlayer Player { get { return inputController.Player; } }

        public bool CarriesOrderAction { get { return OrderAction != null; } }
        public IOrderAction OrderAction { get; set; }
        public MouseState State { get; private set; }
        public MapRaycast MapRaycast { get; private set; }

        public Color FramesColor { get { return BehaviourDeterminant.FramesColor; } }

        public Vector2 Position { get { return Input.mousePosition; } }
        public PlayerMouseButton Left { get; private set; }
        public PlayerMouseButton Right { get; private set; }
        public PlayerMouseButton Middle { get; private set; }
        
        public IMouseBehaviourDeterminant BehaviourDeterminant
        {
            get
            {
                return CarriesOrderAction ? (IMouseBehaviourDeterminant)OrderAction : State;
            }
        }

        public PlayerMouse(InputController inputController)
        {
            this.inputController = inputController;
            hoverController = new HoverController(this) { HoverBoxMinDistance = 5 };

            State = DefaultMouseState.Instance;

            Left = new PlayerMouseButton(0);
            Right = new PlayerMouseButton(1);
            Middle = new PlayerMouseButton(2);
        }

        public void Update()
        {
            UpdateProperties();
            hoverController.Update();

            if (CarriesOrderAction)
            {
            }
            else
            {
                State.Handle(this);
            }
        }

        void UpdateProperties()
        {
            MapRaycast.Update();
            State = inputController.ReadMouseState();

            Left.Update();
            Right.Update();
            Middle.Update();
        }
    }
}

// TODOs:
//   - Merge PlayerMouse, PlayerKeyboard & InputController -> PlayerController
//   - State.Handle() { ??? }
//   - if (CarriesOrderAction) ???
//   - SELECTION