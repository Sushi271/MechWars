using MechWars.Human;
using MechWars.MapElements.Orders.Actions;
using MechWars.PlayerInput.MouseStates;
using UnityEngine;

namespace MechWars.PlayerInput
{
    public class InputController : ICanCreateOrderArgs
    {
        public Player Player { get { return HumanPlayer; } }
        public HumanPlayer HumanPlayer { get; private set; }

        public HoverController HoverController { get; private set; }
        public SelectionMonitor SelectionMonitor { get; private set; }

        public MouseState State { get; private set; }
        public PlayerMouse Mouse { get; private set; }

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

        public bool CarriesOrderAction { get { return CarriedOrderAction != null; } }

        public BuildingShadow BuildingShadow { get; set; }

        public Color FramesColor { get { return BehaviourDeterminant.FramesColor; } }

        public IMouseBehaviourDeterminant BehaviourDeterminant
        {
            get
            {
                return CarriesOrderAction ? (IMouseBehaviourDeterminant)CarriedOrderAction : State;
            }
        }

        public InputController(HumanPlayer player)
        {
            HumanPlayer = player;

            HoverController = new HoverController(this) { HoverBoxMinDistance = 5 };
            SelectionMonitor = new SelectionMonitor();

            State = DefaultMouseState.GetInstance(this);
            Mouse = new PlayerMouse(this);
        }

        public void Update()
        {
            State = ReadMouseState();
            Mouse.Update();
            if (BuildingShadow != null) BuildingShadow.Update();
            HoverController.Update();

            if (CarriesOrderAction) HandleCarriedOrderAction();
            else State.Handle();
        }

        MouseState ReadMouseState()
        {
            if (Input.GetKey(KeyCode.RightAlt)) // it's first, because RightAlt == AltGr == RightControl!!
                return LookAtMouseState.GetInstance(this);
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                return AttackMouseState.GetInstance(this);
            else if (Input.GetKey(KeyCode.LeftAlt))
                return EscortMouseState.GetInstance(this);
            else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                return ToggleSelectMouseState.GetInstance(this);
            else return DefaultMouseState.GetInstance(this);
        }

        bool executeOnUp;
        void HandleCarriedOrderAction()
        {
            // TODO: if SelectedMapElements DIE - cancel if none remaining can do such order

            if (Mouse.Left.IsDown) executeOnUp = true;
            if (Mouse.Right.IsDown)
            {
                CarriedOrderAction = null;
                executeOnUp = false;
            }

            if (executeOnUp && Mouse.Left.IsUp)
            {
                if (SelectionMonitor.SelectedCount == 0)
                    new System.Exception("Game is in invalid state: " +
                        "trying to handle CarriedOrderAction, but no MapElements are selected.");

                if (CarriedOrderAction.AllowsMultiExecutor || SelectionMonitor.SelectedCount == 1)
                {
                    if (CarriedOrderAction.CanCreateOrder(this))
                    {
                        var args = CarriedOrderAction.CreateArgs(this);
                        foreach (var me in SelectionMonitor.SelectedMapElements)
                        {
                            if (me.army != HumanPlayer.Army) continue;

                            var order = CarriedOrderAction.CreateOrder(me, args);
                            me.OrderExecutor.Give(order);
                        }
                    }
                }
                else throw new System.Exception(string.Format(
                    "Game is in invalid state: trying to handle CarriedOrderAction {0}, " +
                    "but it doesn't allow multi-executor and SelectionMonitor.SelectedCount == {1}",
                    CarriedOrderAction.GetType().Name, SelectionMonitor.SelectedCount));
                if (!CarriedOrderAction.IsSequential)
                    CarriedOrderAction = null;
                executeOnUp = false;
            }
        }
    }
}