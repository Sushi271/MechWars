﻿using MechWars.PlayerInput.MouseStates;
using UnityEngine;

namespace MechWars.PlayerInput
{
    public class PlayerMouse
    {
        InputController inputController;

        public Vector2 Position { get { return Input.mousePosition; } }
        public MapRaycast MapRaycast { get; private set; }

        public PlayerMouseButton Left { get; private set; }
        public PlayerMouseButton Right { get; private set; }

        public MouseStateButton MouseStateLeft { get; private set; }
        public MouseStateButton MouseStateRight { get; private set; }

        public bool IsMouseStateButtonActive { get { return MouseStateLeft.WasDown || MouseStateRight.WasDown; } }

        public PlayerMouse(InputController inputController)
        {
            this.inputController = inputController;

            MapRaycast = new MapRaycast(this);

            Left = new PlayerMouseButton(inputController, 0);
            Right = new PlayerMouseButton(inputController, 1);

            MouseStateLeft = new MouseStateButton(inputController, 0);
            MouseStateRight = new MouseStateButton(inputController, 1);
        }

        public void Update()
        {
            MapRaycast.Update();

            Left.Update();
            Right.Update();

            MouseStateLeft.Update();
            MouseStateRight.Update();
        }
    }
}