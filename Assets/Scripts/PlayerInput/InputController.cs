using MechWars.Human;
using MechWars.MapElements;
using MechWars.MapElements.Orders.Actions;
using MechWars.PlayerInput.MouseStates;
using UnityEngine;

namespace MechWars.PlayerInput
{
    public class InputController
    {
        public HumanPlayer Player { get; private set; }

        public HoverController HoverController { get; private set; }
        public SelectionMonitor SelectionMonitor { get; private set; }

        public bool CarriesOrderAction { get { return CarriedOrderAction != null; } }

        OrderAction carriedOrderAction;
        public OrderAction CarriedOrderAction
        {
            get { return carriedOrderAction; }
            set
            {
                if (Mouse.IsMouseStateButtonActive)
                    throw new System.InvalidOperationException(
                        "Cannot assign new CarriedOrderAction while MouseStateButton is active.");
                carriedOrderAction = value;
            }
        }

        public MouseState State { get; private set; }
        public MapRaycast MapRaycast { get; private set; }

        public Color FramesColor { get { return BehaviourDeterminant.FramesColor; } }

        public PlayerMouse Mouse { get; private set; }

        public IMouseBehaviourDeterminant BehaviourDeterminant
        {
            get
            {
                return CarriesOrderAction ? (IMouseBehaviourDeterminant)CarriedOrderAction : State;
            }
        }

        public InputController(HumanPlayer player)
        {
            HoverController = new HoverController(this) { HoverBoxMinDistance = 5 };
            SelectionMonitor = new SelectionMonitor();

            Player = player;

            State = DefaultMouseState.Instance;
        }

        public void Update()
        {
            UpdateProperties();
            HoverController.Update();

            if (CarriesOrderAction) HandleCarriedOrderAction();
            else State.Handle(this);
        }

        void UpdateProperties()
        {
            MapRaycast.Update();
            State = ReadMouseState();

            Mouse.Update();
        }

        MouseState ReadMouseState()
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

        bool executeOnUp;
        void HandleCarriedOrderAction()
        {
            if (Mouse.Left.IsDown) executeOnUp = true;
            if (Mouse.Right.IsDown)
            {
                CarriedOrderAction = null;
                executeOnUp = false;
            }

            if (executeOnUp && Mouse.Left.IsUp)
            {
                var args = new OrderActionArgs(MapRaycast.Coords.Value, HoverController.HoveredMapElements);
                foreach (var me in SelectionMonitor.SelectedMapElements)
                {
                    if (me.army != Player.Army) continue;

                    var order = CarriedOrderAction.CreateOrder(me, args);
                    me.OrderExecutor.Give(order);
                }
                CarriedOrderAction = null;
                executeOnUp = false;
            }
        }
    }
}

// TODOs:
//   - if (CarriesOrderAction) ???
//   - State.Handle() { ??? }