using MechWars.Human;
using MechWars.MapElements.Orders.Actions;
using MechWars.PlayerInput.MouseStates;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MechWars.PlayerInput
{
    public class InputController : ICanCreateOrderArgs
    {
        Dictionary<System.Type, MouseState> mouseStates;

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
        public Color HoverBoxColor { get { return BehaviourDeterminant.HoverBoxColor; } }

        public IMouseBehaviourDeterminant BehaviourDeterminant
        {
            get
            {
                return CarriesOrderAction ? (IMouseBehaviourDeterminant)CarriedOrderAction : State;
            }
        }

        public InputController(HumanPlayer player)
        {
            mouseStates = new Dictionary<System.Type, MouseState>();
            InitializeMouseStates();

            HumanPlayer = player;

            HoverController = new HoverController(this) { HoverBoxMinDistance = 5 };
            SelectionMonitor = new SelectionMonitor();

            State = GetMouseState<DefaultMouseState>();
            Mouse = new PlayerMouse(this);
        }

        public void Update()
        {
            State = ReadMouseState();
            Mouse.Update();
            if (BuildingShadow != null) BuildingShadow.Update();
            HoverController.Update();

            if (CarriesOrderAction) HandleCarriedOrderAction();
            else
            {
                State.Handle();
            }
        }

        MouseState ReadMouseState()
        {
            if (Input.GetKey(KeyCode.RightAlt)) // it's first, because RightAlt == AltGr == RightControl!!
                return GetMouseState<LookAtMouseState>();
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                return GetMouseState<AttackMouseState>();
            else if (Input.GetKey(KeyCode.LeftAlt))
                return GetMouseState<EscortMouseState>();
            else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                return GetMouseState<ToggleSelectMouseState>();
            else return GetMouseState<DefaultMouseState>();
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
                    foreach (var me in SelectionMonitor.SelectedMapElements)
                    {
                        if (me.army != HumanPlayer.Army) continue;

                        CarriedOrderAction.GiveOrder(this, me);
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

        MouseState GetMouseState<T>()
            where T : MouseState
        {
            return mouseStates[typeof(T)];
        }

        void InitializeMouseStates()
        {
            mouseStates.Add(typeof(DefaultMouseState), new DefaultMouseState(this));
            mouseStates.Add(typeof(ToggleSelectMouseState), new ToggleSelectMouseState(this));
            mouseStates.Add(typeof(AttackMouseState), new AttackMouseState(this));
            mouseStates.Add(typeof(EscortMouseState), new EscortMouseState(this));
            mouseStates.Add(typeof(LookAtMouseState), new LookAtMouseState(this));
        }
    }
}