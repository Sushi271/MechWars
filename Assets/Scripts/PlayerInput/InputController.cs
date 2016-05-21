﻿using MechWars.MapElements.Orders.Actions;
using MechWars.PlayerInput.MouseStates;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.PlayerInput
{
    public class InputController
    {
        public Spectator Spectator { get; private set; }

        public HoverController HoverController { get; private set; }
        public SelectionMonitor SelectionMonitor { get; private set; }

        public MouseStateController MouseStateController { get; private set; }
        public PlayerMouse Mouse { get; private set; }

        OrderAction carriedOrderAction;
        public OrderAction CarriedOrderAction
        {
            get { return carriedOrderAction; }
            set
            {
                if (MouseStateController.IsMouseStateButtonActive)
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
                return CarriesOrderAction ?
                    (IMouseBehaviourDeterminant)CarriedOrderAction :
                    MouseStateController.State;
            }
        }

        public IOrderActionArgs OrderActionArgs
        {
            get { return new InputControllerOrderActionArgs(this); }
        }

        public InputController(Spectator spectator)
        {
            Spectator = spectator;

            HoverController = new HoverController(this) { HoverBoxMinDistance = 5 };
            SelectionMonitor = new SelectionMonitor();

            Mouse = new PlayerMouse(this);
            MouseStateController = new MouseStateController(this);
        }

        public void Update()
        {
            Mouse.Update();
            MouseStateController.Update();
            if (BuildingShadow != null) BuildingShadow.Update();
            HoverController.Update();

            if (CarriesOrderAction && Globals.HumanPlayer != null) HandleCarriedOrderAction();
            else MouseStateController.HandleState();
        }

        bool executeOnUp;
        void HandleCarriedOrderAction()
        {
            if (Mouse.Left.IsDown) executeOnUp = true;
            if (Mouse.Right.IsDown || SelectionMonitor.SelectedMapElements.Empty())
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
                        if (me.army != Globals.HumanArmy) continue;

                        CarriedOrderAction.GiveOrder(me, OrderActionArgs);
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