using MechWars.Human;
using MechWars.MapElements.Orders.Actions;
using MechWars.PlayerInput.MouseStates;
using UnityEngine;

namespace MechWars.PlayerInput
{
    public class InputController
    {
        HoverController hoverController;

        public HumanPlayer Player { get; private set; }

        public bool CarriesOrderAction { get { return OrderAction != null; } }
        public IOrderAction OrderAction { get; set; }
        public MouseState State { get; private set; }
        public MapRaycast MapRaycast { get; private set; }

        public Color FramesColor { get { return BehaviourDeterminant.FramesColor; } }

        public PlayerMouse Mouse { get; private set; }
        
        public IMouseBehaviourDeterminant BehaviourDeterminant
        {
            get
            {
                return CarriesOrderAction ? (IMouseBehaviourDeterminant)OrderAction : State;
            }
        }

        public InputController(HumanPlayer player)
        {
            hoverController = new HoverController(this) { HoverBoxMinDistance = 5 };

            Player = player;

            State = DefaultMouseState.Instance;

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
            State = ReadMouseState();

            Mouse.Update();
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

// TODOs:
//   - State.Handle() { ??? }
//   - if (CarriesOrderAction) ???
//   - SELECTION