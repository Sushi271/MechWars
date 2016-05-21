using System;
using System.Collections.Generic;
using UnityEngine;

namespace MechWars.PlayerInput.MouseStates
{
    public class MouseStateController
    {
        Dictionary<System.Type, MouseState> mouseStates;

        public InputController InputController { get; private set; }
        
        public MouseStateButton MouseStateLeft { get; private set; }
        public MouseStateButton MouseStateRight { get; private set; }

        public bool IsMouseStateButtonActive { get { return MouseStateLeft.WasDown || MouseStateRight.WasDown; } }

        public bool LeftActionTriggered { get; private set; }
        public bool RightActionTriggered { get; private set; }

        public MouseState State { get; private set; }

        public MouseStateController(InputController inputController)
        {
            mouseStates = new Dictionary<System.Type, MouseState>();
            InitializeMouseStates();

            InputController = inputController;

            MouseStateLeft = new MouseStateButton(InputController, InputController.Mouse.Left);
            MouseStateRight = new MouseStateButton(InputController, InputController.Mouse.Right);
        }

        public void Update()
        {
            State = ReadMouseState();
            MouseStateLeft.Update();
            MouseStateRight.Update();
            
            UpdateTriggers();
        }

        public void HandleState()
        {
            State.Handle();
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

        bool leftDown;
        private void UpdateTriggers()
        {
            LeftActionTriggered = false;
            RightActionTriggered = false;

            if (MouseStateLeft.IsDown) leftDown = true;
            if (MouseStateRight.IsDown)
                if (leftDown) leftDown = false;
                else RightActionTriggered = true;
            if (leftDown && MouseStateLeft.IsUp)
            {
                LeftActionTriggered = true;
                leftDown = false;
            }

            Debug.Log("A: " + MouseStateLeft.IsDown + " " + MouseStateRight.IsDown);
            Debug.Log("B: " + LeftActionTriggered + " " + RightActionTriggered);
        }
    }
}
