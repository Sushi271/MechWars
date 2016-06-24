using MechWars.MapElements.Orders;
using System.Text;
using UnityEngine;

namespace MechWars.MapElements
{
    public class Unit : MapElement
    {
        protected override bool CanAddToArmy { get { return true; } }
        public override bool Selectable { get { return true; } }
        public override bool CanBeAttacked { get { return true; } }
        public override bool CanBeEscorted { get { return true; } }
        public override bool CanRotateItself { get { return true; } }

        public Move Move { get; private set; }

        protected override OrderQueue CreateOrderQueue(bool enabled = true)
        {
            var orderExecutor = base.CreateOrderQueue();
            orderExecutor.GiveReplaces = true;
            return orderExecutor;
        }

        protected override Sprite GetMarkerImage()
        {
            if (army != null)
                return army.unitMarker;
            return Globals.Textures.neutralUnitMarker;
        }

        protected override float GetMarkerHeight()
        {
            return 3;
        }

        public bool SetMove(SingleMoveOrder singleMoveOrder)
        {
            if (Move != null || singleMoveOrder.State != OrderState.BrandNew)
                return false;
            Move = new Move(singleMoveOrder);
            return true;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (Move != null)
            {
                Move.Update();
                if (Move.Done)
                    Move = null;
            }
        }

        public override StringBuilder DEBUG_PrintStatus(StringBuilder sb)
        {
            base.DEBUG_PrintStatus(sb)
                .AppendLine();
            DEBUG_PrintOrders(sb);
            return sb;
        }
    }
}