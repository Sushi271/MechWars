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

        protected override void InitializeInQuadTree(Army army)
        {
            if (army != null)
                Globals.QuadTreeMap.ArmyQuadTrees[army].Insert(this);
        }

        protected override void FinalizeInQuadTree(Army army)
        {
            if (army != null)
                Globals.QuadTreeMap.ArmyQuadTrees[army].Remove(this);
        }

        protected override void InitializeInVisibilityTable(Army army)
        {
            if (army != null)
                army.VisibilityTable.IncreaseVisibility(this);
        }

        protected override void FinalizeInVisibilityTable(Army army)
        {
            if (army != null)
                army.VisibilityTable.DecreaseVisibility(this);
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