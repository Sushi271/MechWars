using MechWars.PlayerInput;
using UnityEngine;

namespace MechWars.Human
{
    public class HumanPlayer : Player
    {
        public MouseMode MouseMode { get; set; }

        public SelectionController SelectionController { get; private set; }
        public OrderController OrderController { get; private set; }

        public InputController InputController { get; private set; }

        public HumanPlayer()
        {
            SelectionController = new SelectionController(this);
            OrderController = new OrderController(this);

            InputController = new InputController(this);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            //SelectionController.Update();
            //OrderController.Update();
            InputController.Update();
        }
    }
}